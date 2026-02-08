using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionModelTests {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://default",
            DefaultHeaders = {
                new() { Name = "Accept", Value = "application/json" },
                new() { Name = "X-api-key", Value = "4p1-k3y" }
            }
        };
        var modelDataProvider = Substitute.For<IModelDataProvider<(string, RequestTemplateCollection)>>();
        modelDataProvider.GetData().Returns((filePath, collection));

        var subject = new RequestTemplateCollectionModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IMessageService>(), modelDataProvider);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - Collection settings", subject.PageTitle);
        Assert.Equal("Collection settings", subject.ShellItemTitle);
        Assert.Equal(collection.DefaultUrl, subject.DefaultUrl.Value);
        Assert.Equal(collection.DefaultHeaders.Count, subject.DefaultHeaders.Count);

        foreach (var header in collection.DefaultHeaders) {
            Assert.Equal(header.Value, Assert.Single(subject.DefaultHeaders, headerModel => headerModel.Name.Value == header.Name).Value.Value);
        }
    }

    [Theory]
    [InlineData(false, false, "External data requests - Collection settings", "Collection settings")]
    [InlineData(true, false, "External data requests - Collection settings ▲", "Collection settings ▲")]
    [InlineData(false, true, "External data requests - Collection settings ●", "Collection settings ●")]
    [InlineData(true, true, "External data requests - Collection settings ▲", "Collection settings ▲")]
    public void UpdateTitles(bool hasError, bool isModified, string expectedPageTitle, string expectedShellItemTitle) {
        var modelDataProvider = Substitute.For<IModelDataProvider<(string, RequestTemplateCollection)>>();
        modelDataProvider.GetData().Returns((@"C:\Documents\External data requests.json", new()));

        var subject = new RequestTemplateCollectionModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IMessageService>(), modelDataProvider) {
            HasError = hasError,
            IsModified = isModified
        };

        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }

    [Fact]
    public async Task Update() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string defaultUrl = "https://default";
        const string headerName = "Name";
        const string headerValue = "Value";
        const string variableName = "Name";
        const string variableValue = "Value";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            DefaultHeaders = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            Variables = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };
        var modelDataProvider = Substitute.For<IModelDataProvider<(string, RequestTemplateCollection)>>();
        modelDataProvider.GetData().Returns((filePath, collection));

        var subject = new RequestTemplateCollectionModel(requestTemplateCollectionService, messageService, modelDataProvider);

        subject.DefaultUrl.Value = defaultUrl;
        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.DefaultHeaders[0].Value.Value = headerValue;
        subject.Variables[0].Name.Value = variableName;
        subject.Variables[0].Value.Value = variableValue;

        await subject.Update();

        Assert.Equal(defaultUrl, collection.DefaultUrl);
        Assert.Equal(headerName, collection.DefaultHeaders[0].Name);
        Assert.Equal(headerValue, collection.DefaultHeaders[0].Value);
        Assert.Equal(variableName, collection.Variables[0].Name);
        Assert.Equal(variableValue, collection.Variables[0].Value);
        Assert.False(subject.DefaultUrl.IsModified);
        Assert.False(subject.DefaultHeaders[0].Name.IsModified);
        Assert.False(subject.DefaultHeaders[0].Value.IsModified);
        Assert.False(subject.Variables[0].Name.IsModified);
        Assert.False(subject.Variables[0].Value.IsModified);

        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData("Name", null, "Name", "Value")]
    [InlineData(null, "Value", "Name", "Value")]
    [InlineData("Name", "Value", null, "Value")]
    [InlineData("Name", "Value", "Name", null)]
    public async Task Update_Fails_When_Invalid(string? headerName, string? headerValue, string? variableName, string? variableValue) {
        const string defaultUrl = "https://default";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            DefaultHeaders = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            Variables = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };
        var modelDataProvider = Substitute.For<IModelDataProvider<(string, RequestTemplateCollection)>>();
        modelDataProvider.GetData().Returns((@"C:\Documents\External data requests.json", collection));

        var subject = new RequestTemplateCollectionModel(requestTemplateCollectionService, messageService, modelDataProvider);

        subject.DefaultUrl.Value = defaultUrl;
        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.DefaultHeaders[0].Value.Value = headerValue;
        subject.Variables[0].Name.Value = variableName;
        subject.Variables[0].Value.Value = variableValue;

        await subject.Update();

        Assert.Equal("https://previous", collection.DefaultUrl);
        Assert.Equal("PreviousName", collection.DefaultHeaders[0].Name);
        Assert.Equal("PreviousValue", collection.DefaultHeaders[0].Value);
        Assert.Equal("PreviousName", collection.Variables[0].Name);
        Assert.Equal("PreviousValue", collection.Variables[0].Value);

        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
