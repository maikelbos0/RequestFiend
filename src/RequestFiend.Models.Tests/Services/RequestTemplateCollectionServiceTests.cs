using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests.Services;

public class RequestTemplateCollectionServiceTests {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string title = "External data requests";

        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.Path.GetFileNameWithoutExtension(filePath).Returns(title);
        var collection = new RequestTemplateCollection();
        var requestTemplateCollectionProvider = Substitute.For<IRequestTemplateCollectionProvider>();
        requestTemplateCollectionProvider.GetData().Returns((filePath, collection));

        var subject = new RequestTemplateCollectionService(Substitute.For<IMessageService>(), fileSystem, requestTemplateCollectionProvider);

        Assert.Equal("External data requests", subject.Title);
        Assert.Equal(collection, subject.Collection);
    }

    [Fact]
    public async Task SaveCollection() {
        const string filePath = @"C:\Documents\External data requests.json";

        var fileSystem = Substitute.For<IFileSystem>();
        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection() {
            Variables = { new() { Name = "DefaultHeader", Value = "Accept" } },
            DefaultHeaders = [
                new() { Name = "{{DefaultHeader}}", Value = "application/json" }
            ]
        };
        var requestTemplateCollectionProvider = Substitute.For<IRequestTemplateCollectionProvider>();
        requestTemplateCollectionProvider.GetData().Returns((filePath, collection));

        var subject = new RequestTemplateCollectionService(messageService, fileSystem, requestTemplateCollectionProvider);

        await subject.Save();

        await fileSystem.Received(1).File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));
        messageService.Received(1).Send(Arg.Is<RequestTemplateCollectionUpdatedMessage>(x => x.FilePath == filePath && x.Collection == collection));
        messageService.Received(1).Send(Arg.Is<RequestTemplateCollectionUpdatedMessage>(x => x.FilePath == filePath && x.Collection == collection), filePath);
    }
}
