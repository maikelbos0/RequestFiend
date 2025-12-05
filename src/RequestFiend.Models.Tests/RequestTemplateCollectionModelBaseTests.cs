using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionModelBaseTests {
    [Fact]
    public void Constructor() {
        var subject = new RequestTemplateCollectionModelBase(Substitute.For<IFileService>(), @"C:\Documents\External data requests.json", new());

        Assert.Equal("External data requests", subject.Title);
    }

    [Fact]
    public async Task SaveCollection() {
        const string filePath = @"C:\Documents\External data requests.json";

        var fileService = Substitute.For<IFileService>();
        var collection = new RequestTemplateCollection() {
            Variables = { new() { Name = "DefaultHeader", Value = "Accept" } },
            DefaultHeaders = [
                new() { Name = "{{DefaultHeader}}", Value = "application/json" }
            ]
        };

        var subject = new RequestTemplateCollectionModelBase(fileService, filePath, collection);

        await subject.SaveCollection();

        await fileService.Received(1).WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));
    }
}
