using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionSettingsModelTests {
    [Theory]
    [InlineData(ScriptEvaluationMode.Disabled, false)]
    [InlineData(ScriptEvaluationMode.Enabled, false)]
    [InlineData(ScriptEvaluationMode.CollectionScoped, true)]
    public void Constructor(ScriptEvaluationMode scriptEvaluationMode, bool expectedShowAllowScriptEvaluation) {
        const string filePath = @"C:\Documents\External data requests.json";
        const bool allowScriptEvaluation = true;

        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetScriptEvaluationMode().Returns(scriptEvaluationMode);
        preferencesService.GetCollectionAllowScriptEvaluation(filePath).Returns(allowScriptEvaluation);
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://default",
            DefaultHeaders = {
                new() { Name = "Accept", Value = "application/json" },
                new() { Name = "X-api-key", Value = "4p1-k3y" }
            }
        };

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            messageService,
            preferencesService,
            new(filePath),
            collection
        );

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - Collection settings", subject.PageTitleBase);
        Assert.Equal("Collection settings", subject.ShellItemTitleBase);

        Assert.Equal(new RequestTemplateCollectionFileModel(filePath), subject.File);
        Assert.Equal(collection, subject.Collection);

        Assert.Equal(expectedShowAllowScriptEvaluation, subject.ShowAllowScriptEvaluation);
        Assert.Equal(allowScriptEvaluation, subject.AllowScriptEvaluation.Value);
        Assert.Equal(collection.DefaultUrl, subject.DefaultUrl.Value);
        Assert.Equal(collection.DefaultHeaders.Count, subject.DefaultHeaders.Count);

        foreach (var header in collection.DefaultHeaders) {
            Assert.Equal(header.Value, Assert.Single(subject.DefaultHeaders, headerModel => headerModel.Name.Value == header.Name).Value.Value);
        }

        messageService.Received(1).Register(subject, Arg.Any<MessageHandler<RequestTemplateCollectionSettingsModel, PreferencesUpdatedMessage>>());
    }

    [Fact]
    public async Task Update() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string defaultUrl = "https://default";
        const string headerName = "Name";
        const string headerValue = "Value";
        const string variableName = "Name";
        const string variableValue = "Value";
        const bool allowScriptEvaluation = true;

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetCollectionAllowScriptEvaluation(filePath).Returns(!allowScriptEvaluation);
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            Variables = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            DefaultHeaders = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };

        var subject = new RequestTemplateCollectionSettingsModel(
            requestTemplateCollectionService,
            Substitute.For<IPopupService>(),
            messageService,
            preferencesService,
            new(filePath),
            collection
        );

        subject.AllowScriptEvaluation.Value = allowScriptEvaluation;
        subject.DefaultUrl.Value = defaultUrl;
        subject.Variables[0].Name.Value = variableName;
        subject.Variables[0].Value.Value = variableValue;
        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.DefaultHeaders[0].Value.Value = headerValue;

        await subject.Update();

        Assert.Equal(defaultUrl, collection.DefaultUrl);
        Assert.Equal(variableName, collection.Variables[0].Name);
        Assert.Equal(variableValue, collection.Variables[0].Value);
        Assert.Equal(headerName, collection.DefaultHeaders[0].Name);
        Assert.Equal(headerValue, collection.DefaultHeaders[0].Value);
        Assert.False(subject.AllowScriptEvaluation.IsModified);
        Assert.False(subject.DefaultUrl.IsModified);
        Assert.False(subject.DefaultHeaders[0].Name.IsModified);
        Assert.False(subject.DefaultHeaders[0].Value.IsModified);
        Assert.False(subject.Variables[0].Name.IsModified);
        Assert.False(subject.Variables[0].Value.IsModified);

        preferencesService.Received(1).SetCollectionAllowScriptEvaluation(filePath, allowScriptEvaluation);
        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
        messageService.Received(1).Send(Arg.Is<RequestTemplateCollectionSettingsUpdatedMessage>(x => x.Collection == collection));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("", "Name")]
    [InlineData("Name", "")]
    public async Task Update_Fails_When_Invalid(string headerName, string variableName) {
        const string filePath = @"C:\Documents\External data requests.json";
        const string defaultUrl = "https://default";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            Variables = {
                new() { Name = "PreviousName" }
            },
            DefaultHeaders = {
                new() { Name = "PreviousName" }
            }
        };

        var subject = new RequestTemplateCollectionSettingsModel(
            requestTemplateCollectionService,
            Substitute.For<IPopupService>(),
            messageService,
            preferencesService,
            new(filePath),
            collection
        );

        subject.DefaultUrl.Value = defaultUrl;
        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.Variables[0].Name.Value = variableName;

        await subject.Update();

        Assert.Equal("https://previous", collection.DefaultUrl);
        Assert.Equal("PreviousName", collection.Variables[0].Name);
        Assert.Equal("PreviousName", collection.DefaultHeaders[0].Name);

        preferencesService.DidNotReceive().SetCollectionAllowScriptEvaluation(Arg.Any<string>(), Arg.Any<bool>());
        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        messageService.DidNotReceive().Send(Arg.Any<RequestTemplateCollectionSettingsUpdatedMessage>());
    }

    [Fact]
    public void ToggleShowRecentCollections() {
        const string filePath = @"C:\Documents\External data requests.json";

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            new()
        ) {
            AllowScriptEvaluation = { Value = true },
        };

        subject.ToggleAllowScriptEvaluation();

        Assert.False(subject.AllowScriptEvaluation.Value);
    }

    [Theory]
    [InlineData(null, "https://localhost")]
    [InlineData("https://localhost/api", "https://localhost/api")]
    public async Task ShowDefaultUrlPopup(string? returnValue, string expectedUrl) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://localhost"
        };
        popupResult.Result.Returns(returnValue);
        popupService.ShowUrlPopup(collection, collection.DefaultUrl).Returns(popupResult);

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            popupService,
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        );

        await subject.ShowDefaultUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, collection.DefaultUrl);
        Assert.Equal(expectedUrl, subject.DefaultUrl.Value);
    }
}
