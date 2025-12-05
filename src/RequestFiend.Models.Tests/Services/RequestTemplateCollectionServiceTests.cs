using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Media;
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

        var subject = new RequestTemplateCollectionService(Substitute.For<IMessageService>(), fileSystem, (filePath, new()));

        Assert.Equal("External data requests", subject.Title);
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

        var subject = new RequestTemplateCollectionService(messageService, fileSystem, (filePath, collection));

        await subject.Save();

        await fileSystem.Received(1).File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));
        messageService.Received(1).Send(Arg.Is<RequestTemplateCollectionUpdatedMessage>(x => x.FilePath == filePath && x.Collection == collection));
        messageService.Received(1).Send(Arg.Is<RequestTemplateCollectionUpdatedMessage>(x => x.FilePath == filePath && x.Collection == collection), filePath);
    }
}
