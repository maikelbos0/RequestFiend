using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionSettingsModelTests {
    [Fact]
    public void DefaultUrl() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        ) {
            DefaultUrl = { Value = "https://default" }
        };

        Assert.Equal("https://default", collection.DefaultUrl);
    }

    [Fact]
    public void IgnoreRemoteCertificateNotAvailable() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        ) {
            IgnoreRemoteCertificateNameMismatch = { Value = true }
        };

        Assert.True(collection.IgnoreRemoteCertificateNameMismatch);
    }

    [Fact]
    public void IgnoreRemoteCertificateNameMismatch() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        ) {
            IgnoreRemoteCertificateNameMismatch = { Value = true }
        };

        Assert.True(collection.IgnoreRemoteCertificateNameMismatch);
    }

    [Fact]
    public void IgnoreRemoteCertificateChainErrors() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        ) {
            IgnoreRemoteCertificateChainErrors = { Value = true }
        };

        Assert.True(collection.IgnoreRemoteCertificateChainErrors);
    }

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
            IgnoreRemoteCertificateNotAvailable = true,
            IgnoreRemoteCertificateNameMismatch = true,
            IgnoreRemoteCertificateChainErrors = true,
            Variables = {
                new() { Name = "Foo", Value = "Bar" }
            },
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

        Assert.Equal(new FileModel(filePath), subject.File);
        Assert.Equal(collection, subject.Collection);

        Assert.Equal(expectedShowAllowScriptEvaluation, subject.ShowAllowScriptEvaluation);
        Assert.Equal(allowScriptEvaluation, subject.AllowScriptEvaluation.Value);
        Assert.Equal(collection.DefaultUrl, subject.DefaultUrl.Value);
        Assert.Equal(collection.IgnoreRemoteCertificateNotAvailable, subject.IgnoreRemoteCertificateNotAvailable.Value);
        Assert.Equal(collection.IgnoreRemoteCertificateNameMismatch, subject.IgnoreRemoteCertificateNameMismatch.Value);
        Assert.Equal(collection.IgnoreRemoteCertificateChainErrors, subject.IgnoreRemoteCertificateChainErrors.Value);
        Assert.Equal(collection.Variables.Count, subject.Variables.Count);
        Assert.Equal(collection.DefaultHeaders.Count, subject.DefaultHeaders.Count);

        messageService.Received(1).Register(subject, Arg.Any<MessageHandler<RequestTemplateCollectionSettingsModel, PreferencesUpdatedMessage>>());

        Assert.Equal([
            subject.AllowScriptEvaluation,
            subject.DefaultUrl,
            subject.IgnoreRemoteCertificateNotAvailable,
            subject.IgnoreRemoteCertificateNameMismatch,
            subject.IgnoreRemoteCertificateChainErrors,
            subject.Variables,
            subject.DefaultHeaders
        ], subject.Validatables);
    }

    [Fact]
    public async Task Update() {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetCollectionAllowScriptEvaluation(filePath).Returns(false);
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            IgnoreRemoteCertificateNotAvailable = false,
            IgnoreRemoteCertificateNameMismatch = false,
            IgnoreRemoteCertificateChainErrors = false,
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

        subject.AllowScriptEvaluation.Value = true;
        subject.DefaultUrl.Value = "https://default";
        subject.IgnoreRemoteCertificateNotAvailable.Value = true;
        subject.IgnoreRemoteCertificateNameMismatch.Value = true;
        subject.IgnoreRemoteCertificateChainErrors.Value = true;
        subject.Variables[0].Name.Value = "Name";
        subject.Variables[0].Value.Value = "Value";
        subject.DefaultHeaders[0].Name.Value = "Name";
        subject.DefaultHeaders[0].Value.Value = "Value";

        await subject.Update();

        Assert.False(subject.AllowScriptEvaluation.IsModified);
        Assert.False(subject.DefaultUrl.IsModified);
        Assert.False(subject.IgnoreRemoteCertificateNotAvailable.IsModified);
        Assert.False(subject.IgnoreRemoteCertificateNameMismatch.IsModified);
        Assert.False(subject.IgnoreRemoteCertificateChainErrors.IsModified);
        Assert.False(subject.DefaultHeaders.IsModified);
        Assert.False(subject.Variables.IsModified);

        preferencesService.Received(1).SetCollectionAllowScriptEvaluation(filePath, true);
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

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            IgnoreRemoteCertificateNotAvailable = false,
            IgnoreRemoteCertificateNameMismatch = false,
            IgnoreRemoteCertificateChainErrors = false,
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

        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.Variables[0].Name.Value = variableName;

        await subject.Update();

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

    [Fact]
    public void ToggleIgnoreRemoteCertificateNotAvailable() {
        const string filePath = @"C:\Documents\External data requests.json";

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            new()
        ) {
            IgnoreRemoteCertificateNotAvailable = { Value = true },
        };

        subject.ToggleIgnoreRemoteCertificateNotAvailable();

        Assert.False(subject.IgnoreRemoteCertificateNotAvailable.Value);
    }

    [Fact]
    public void ToggleIgnoreRemoteCertificateNameMismatch() {
        const string filePath = @"C:\Documents\External data requests.json";

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            new()
        ) {
            IgnoreRemoteCertificateNameMismatch = { Value = true },
        };

        subject.ToggleIgnoreRemoteCertificateNameMismatch();

        Assert.False(subject.IgnoreRemoteCertificateNameMismatch.Value);
    }

    [Fact]
    public void ToggleIgnoreRemoteCertificateChainErrors() {
        const string filePath = @"C:\Documents\External data requests.json";

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            new()
        ) {
            IgnoreRemoteCertificateChainErrors = { Value = true },
        };

        subject.ToggleIgnoreRemoteCertificateChainErrors();

        Assert.False(subject.IgnoreRemoteCertificateChainErrors.Value);
    }

    [Fact]
    public async Task ShowDefaultUrlPopup() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string defaultUrl = "https://localhost";
        const string expectedUrl = "https://localhost/api";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = defaultUrl
        };
        popupResult.Result.Returns(expectedUrl);
        popupService.ShowUrlPopup(collection, defaultUrl).Returns(popupResult);
        var messageService = Substitute.For<IMessageService>();

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            popupService,
            messageService,
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        );

        await subject.ShowDefaultUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, defaultUrl);
        Assert.Equal(expectedUrl, subject.DefaultUrl.Value);
        messageService.Received(1).Send(Arg.Is<ValidatablePropertyUpdatedMessage>(message => message.Property == subject.DefaultUrl));
    }

    [Fact]
    public async Task ShowDefaultUrlPopup_Does_Nothing_Without_Result() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string defaultUrl = "https://localhost";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = defaultUrl
        };
        popupResult.Result.ReturnsNull();
        popupService.ShowUrlPopup(collection, defaultUrl).Returns(popupResult);
        var messageService = Substitute.For<IMessageService>();

        var subject = new RequestTemplateCollectionSettingsModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            popupService,
            messageService,
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        );

        await subject.ShowDefaultUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, defaultUrl);
        Assert.Equal(defaultUrl, subject.DefaultUrl.Value);
        messageService.DidNotReceive().Send(Arg.Any<ValidatablePropertyUpdatedMessage>());
    }
}
