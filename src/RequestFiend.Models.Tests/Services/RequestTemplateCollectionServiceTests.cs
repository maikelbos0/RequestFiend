using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests.Services;

public class RequestTemplateCollectionServiceTests {
    [Fact]
    public async Task Save() {
        const string filePath = @"C:\Documents\External data requests.json";

        var fileSystem = Substitute.For<IFileSystem>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var collection = new RequestTemplateCollection() {
            Variables = { new() { Name = "DefaultHeader", Value = "Accept" } },
            DefaultHeaders = [
                new() { Name = "{{DefaultHeader}}", Value = "application/json" }
            ]
        };

        var subject = new RequestTemplateCollectionService(messageService, fileSystem, preferencesService);

        await subject.Save(filePath, collection);

        await fileSystem.Received(1).File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection), Arg.Any<CancellationToken>());
        messageService.Received(1).Send(Arg.Any<RequestTemplateCollectionUpdatedMessage>(), filePath);
        preferencesService.Received(1).PushRecentCollection(filePath);
    }
}
