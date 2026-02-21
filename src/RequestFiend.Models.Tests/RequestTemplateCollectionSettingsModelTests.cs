using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionSettingsModelTests {
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
        
        var subject = new RequestTemplateCollectionSettingsModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IMessageService>(), new(filePath), collection);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - Collection settings", subject.PageTitleBase);
        Assert.Equal("Collection settings", subject.ShellItemTitleBase);
        Assert.Equal(collection.DefaultUrl, subject.DefaultUrl.Value);
        Assert.Equal(collection.DefaultHeaders.Count, subject.DefaultHeaders.Count);

        foreach (var header in collection.DefaultHeaders) {
            Assert.Equal(header.Value, Assert.Single(subject.DefaultHeaders, headerModel => headerModel.Name.Value == header.Name).Value.Value);
        }
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

        var subject = new RequestTemplateCollectionSettingsModel(requestTemplateCollectionService, messageService, new(filePath), collection);

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
    [InlineData("", "", "", "")]
    [InlineData("Name", "", "Name", "Value")]
    [InlineData("", "Value", "Name", "Value")]
    [InlineData("Name", "Value", "", "Value")]
    [InlineData("Name", "Value", "Name", "")]
    public async Task Update_Fails_When_Invalid(string headerName, string headerValue, string variableName, string variableValue) {
        const string filePath = @"C:\Documents\External data requests.json";
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

        var subject = new RequestTemplateCollectionSettingsModel(requestTemplateCollectionService, messageService, new(filePath), collection);

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
