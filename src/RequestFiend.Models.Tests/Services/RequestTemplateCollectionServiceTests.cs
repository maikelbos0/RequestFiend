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
    public async Task Save() {
        const string filePath = @"C:\Documents\External data requests.json";

        var fileSystem = Substitute.For<IFileSystem>();
        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection() {
            Variables = { new() { Name = "DefaultHeader", Value = "Accept" } },
            DefaultHeaders = [
                new() { Name = "{{DefaultHeader}}", Value = "application/json" }
            ]
        };
        var subject = new RequestTemplateCollectionService(messageService, fileSystem);

        await subject.Save(filePath, collection);

        await fileSystem.Received(1).File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));
        messageService.Received(1).Send(Arg.Is<RequestTemplateCollectionUpdatedMessage>(x => x.FilePath == filePath && x.Collection == collection));
        messageService.Received(1).Send(Arg.Is<RequestTemplateCollectionUpdatedMessage>(x => x.FilePath == filePath && x.Collection == collection), filePath);
    }
}
