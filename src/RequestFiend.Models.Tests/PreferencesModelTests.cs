using CommunityToolkit.Maui.Storage;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Storage = Microsoft.Maui.Storage;

namespace RequestFiend.Models.Tests;

public class PreferencesModelTests {
    [Theory]
    [InlineData("", 0)]
    [InlineData("0", 0)]
    [InlineData("1", 1)]
    [InlineData("10", 10)]
    public void MaximumRecentCollectionCount(string maximumRecentCollectionCount, int expectedMaximumRecentCollectionCount) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount }
        };

        subject.MaximumRecentCollectionCount.Set();

        preferencesService.Received().SetMaximumRecentCollectionCount(expectedMaximumRecentCollectionCount);
    }

    [Fact]
    public void ScriptEvaluationMode() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[Models.ScriptEvaluationMode.Enabled] }
        };

        subject.ScriptEvaluationMode.Set();

        preferencesService.Received().SetScriptEvaluationMode(Models.ScriptEvaluationMode.Enabled);
    }

    [Theory]
    [InlineData("10", 10)]
    [InlineData("1", 1)]
    [InlineData("0", 0)]
    [InlineData("", null)]
    public void RequestTimeoutInSeconds(string requestTimeoutInSeconds, int? expectedRequestTimeoutInSeconds) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            RequestTimeoutInSeconds = { Value = requestTimeoutInSeconds }
        };

        subject.RequestTimeoutInSeconds.Set();

        preferencesService.Received().SetRequestTimeoutInSeconds(expectedRequestTimeoutInSeconds);
    }

    [Fact]
    public void ExchangeLoggingPath() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            ExchangeLoggingPath = { Value = "./Location" }
        };

        subject.ExchangeLoggingPath.Set();

        preferencesService.Received().SetExchangeLoggingPath("./Location");
    }

    [Fact]
    public void ExchangeLoggingOutputTemplate() {
        const string exchangeLoggingOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            ExchangeLoggingOutputTemplate = { Value = exchangeLoggingOutputTemplate }
        };

        subject.ExchangeLoggingOutputTemplate.Set();

        preferencesService.Received().SetExchangeLoggingOutputTemplate(exchangeLoggingOutputTemplate);
    }

    [Fact]
    public void ActiveEnvironment() {
        const string activeEnvironment = @"C:\Documents\Environment.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            ActiveEnvironment = { Value = new(activeEnvironment) }
        };

        subject.ActiveEnvironment.Set();

        preferencesService.Received().SetActiveEnvironment(new(activeEnvironment));
    }

    [Theory]
    [InlineData(Models.ScriptEvaluationMode.Disabled)]
    [InlineData(Models.ScriptEvaluationMode.Enabled)]
    [InlineData(Models.ScriptEvaluationMode.CollectionScoped)]
    public void Constructor(ScriptEvaluationMode scriptEvaluationMode) {
        const int maximumRecentCollectionCount = 10;
        const int requestTimeoutInSeconds = 90;
        const string exchangeLoggingPath = "./Path";
        const string exchangeLoggingOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        const string activeEnvironment = @"C:\Documents\Foo.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);
        preferencesService.GetScriptEvaluationMode().Returns(scriptEvaluationMode);
        preferencesService.GetRequestTimeoutInSeconds().Returns(requestTimeoutInSeconds);
        preferencesService.GetExchangeLoggingPath().Returns(exchangeLoggingPath);
        preferencesService.GetExchangeLoggingOutputTemplate().Returns(exchangeLoggingOutputTemplate);
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(@"C:\Documents\Bar.json"), new(@"C:\Documents\Bar.json")]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        Assert.Equal("Preferences", subject.PageTitleBase);
        Assert.Equal("Preferences", subject.ShellItemTitleBase);

        Assert.Equal(maximumRecentCollectionCount.ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[scriptEvaluationMode], subject.ScriptEvaluationMode.Value);
        Assert.Equal(requestTimeoutInSeconds.ToString(), subject.RequestTimeoutInSeconds.Value);
        Assert.Equal(exchangeLoggingPath, subject.ExchangeLoggingPath.Value);
        Assert.Equal(exchangeLoggingOutputTemplate, subject.ExchangeLoggingOutputTemplate.Value);
        Assert.Equal(preferencesService.GetEnvironments().Distinct().OrderBy(environment => environment.Name, System.StringComparer.CurrentCultureIgnoreCase), subject.Environments);
        Assert.Equal(new(activeEnvironment), subject.ActiveEnvironment.Value);
        Assert.Contains(subject.Environments, environment => ReferenceEquals(environment, subject.ActiveEnvironment.Value));

        Assert.Equal([
            subject.RequestTimeoutInSeconds,
            subject.MaximumRecentCollectionCount,
            subject.ScriptEvaluationMode,
            subject.ExchangeLoggingPath,
            subject.ExchangeLoggingOutputTemplate,
            subject.Environments,
            subject.ActiveEnvironment
        ], subject.Validatables);
    }

    [Fact]
    public void Update() {
        const string existingEnvironment = @"C:\Documents\Foo.json";
        const string newEnvironment = @"C:\Documents\Bar.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(existingEnvironment)]);
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            Environments = { new(newEnvironment) }
        };

        subject.Update();

        preferencesService.Received(1).SetEnvironments(Arg.Is<ValidatableImmutableCollection<FileModel>>(collection => collection.SequenceEqual(new FileModel[] { new(existingEnvironment), new(newEnvironment) })));

        Assert.False(subject.IsModified);

        preferencesService.Received(1).TrimRecentCollections();

        messageService.Received(1).Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("Invalid", "10")]
    [InlineData("10", "Invalid")]
    public void Update_Fails_When_Invalid(string maximumRecentCollectionCount, string requestTimeoutInSeconds) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Environment.json")]);
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount },
            RequestTimeoutInSeconds = { Value = requestTimeoutInSeconds }
        };

        subject.Update();

        preferencesService.DidNotReceive().SetEnvironments(Arg.Any<IEnumerable<FileModel>>());

        Assert.True(subject.IsModified);

        preferencesService.DidNotReceive().TrimRecentCollections();

        messageService.DidNotReceive().Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task CreateNewEnvironment() {
        const string activeEnvironment = @"C:\Documents\Foo.json";
        const string otherEnvironment = @"C:\Documents\Bar.json";
        const string newEnvironment = @"C:\Documents\Baz.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(otherEnvironment)]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(newEnvironment, null));
        var environmentService = Substitute.For<IEnvironmentService>();

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), environmentService);

        await subject.CreateNewEnvironment();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new Environment())));
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
        await popupService.Received(1).ShowEnvironmentPopup(environmentService, new(newEnvironment), Arg.Any<Environment>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
        Assert.Equal([new(otherEnvironment), new(newEnvironment), new(activeEnvironment)], subject.Environments);
        Assert.Equal(new(newEnvironment), subject.ActiveEnvironment.Value);
    }

    [Fact]
    public async Task CreateNewEnvironment_Works_When_Empty() {
        const string newEnvironment = @"C:\Documents\Baz.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(newEnvironment, null));
        var environmentService = Substitute.For<IEnvironmentService>();

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), environmentService);

        await subject.CreateNewEnvironment();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new Environment())));
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
        await popupService.Received(1).ShowEnvironmentPopup(environmentService, new(newEnvironment), Arg.Any<Environment>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
        Assert.Equal([new(newEnvironment)], subject.Environments);
        Assert.Equal(new(newEnvironment), subject.ActiveEnvironment.Value);
    }

    [Fact]
    public async Task CreateNewEnvironment_Fails_For_Invalid_FileSaverResult() {
        const string activeEnvironment = @"C:\Documents\Foo.json";
        const string otherEnvironment = @"C:\Documents\Bar.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(otherEnvironment)]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, new System.Exception()));

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        await subject.CreateNewEnvironment();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new Environment())));
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        await popupService.DidNotReceive().ShowEnvironmentPopup(Arg.Any<IEnvironmentService>(), Arg.Any<FileModel>(), Arg.Any<Environment>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
        Assert.Equal([new(otherEnvironment), new(activeEnvironment)], subject.Environments);
        Assert.Equal(new(activeEnvironment), subject.ActiveEnvironment.Value);
    }

    [Fact]
    public async Task CreateNewEnvironment_Does_Nothing_When_Canceled() {
        const string activeEnvironment = @"C:\Documents\Foo.json";
        const string otherEnvironment = @"C:\Documents\Bar.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(otherEnvironment)]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, new System.OperationCanceledException()));

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        await subject.CreateNewEnvironment();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new Environment())));
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        await popupService.DidNotReceive().ShowEnvironmentPopup(Arg.Any<IEnvironmentService>(), Arg.Any<FileModel>(), Arg.Any<Environment>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
        Assert.Equal([new(otherEnvironment), new(activeEnvironment)], subject.Environments);
        Assert.Equal(new(activeEnvironment), subject.ActiveEnvironment.Value);
    }

    [Fact]
    public async Task OpenExistingEnvironment() {
        const string activeEnvironment = @"C:\Documents\Foo.json";
        const string otherEnvironment = @"C:\Documents\Bar.json";
        const string newEnvironment = @"C:\Documents\Baz.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(otherEnvironment)]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(newEnvironment));

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        await subject.OpenExistingEnvironment();

        Assert.Equal([new(otherEnvironment), new(newEnvironment), new(activeEnvironment)], subject.Environments);
        Assert.Equal(new(newEnvironment), subject.ActiveEnvironment.Value);
    }

    [Fact]
    public async Task OpenExistingEnvironment_Works_When_Empty() {
        const string newEnvironment = @"C:\Documents\Baz.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(newEnvironment));

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        await subject.OpenExistingEnvironment();

        Assert.Equal([new(newEnvironment)], subject.Environments);
        Assert.Equal(new(newEnvironment), subject.ActiveEnvironment.Value);
    }

    [Fact]
    public async Task OpenExistingEnvironment_Does_Nothing_Without_Selected_File() {
        const string activeEnvironment = @"C:\Documents\Foo.json";
        const string otherEnvironment = @"C:\Documents\Bar.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(otherEnvironment)]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).ReturnsNull();

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        await subject.OpenExistingEnvironment();

        Assert.Equal([new(otherEnvironment), new(activeEnvironment)], subject.Environments);
        Assert.Equal(new(activeEnvironment), subject.ActiveEnvironment.Value);
    }

    [Fact]
    public void RemoveEnvironment() {
        const string activeEnvironment = @"C:\Documents\Foo.json";
        const string otherEnvironment = @"C:\Documents\Bar.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(otherEnvironment)]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        subject.RemoveEnvironment(new(otherEnvironment));

        Assert.Equal([new(activeEnvironment)], subject.Environments);
        Assert.Equal(new(activeEnvironment), subject.ActiveEnvironment.Value);
    }

    [Fact]
    public void RemoveEnvironment_Clears_ActiveEnvironment_If_Removed() {
        const string activeEnvironment = @"C:\Documents\Foo.json";
        const string otherEnvironment = @"C:\Documents\Bar.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(otherEnvironment)]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        subject.RemoveEnvironment(new(activeEnvironment));

        Assert.Equal([new(otherEnvironment)], subject.Environments);
        Assert.Null(subject.ActiveEnvironment.Value);
    }

    [Fact]
    public async Task OpenEnvironmentPopup() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns(JsonSerializer.Serialize(new Environment()));
        var environmentService = Substitute.For<IEnvironmentService>();

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, fileSystem, environmentService);

        await subject.OpenEnvironmentPopup(new(filePath));

        await popupService.Received(1).ShowEnvironmentPopup(environmentService, new(filePath), Arg.Any<Environment>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenEnvironmentPopup_Fails_For_Missing_File() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(false);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, fileSystem, Substitute.For<IEnvironmentService>());

        await subject.OpenEnvironmentPopup(new(filePath));

        await popupService.DidNotReceive().ShowEnvironmentPopup(Arg.Any<IEnvironmentService>(), Arg.Any<FileModel>(), Arg.Any<Environment>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenEnvironmentPopup_Fails_For_Deserialization_Error() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("Invalid JSON");

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, fileSystem, Substitute.For<IEnvironmentService>());

        await subject.OpenEnvironmentPopup(new(filePath));

        await popupService.DidNotReceive().ShowEnvironmentPopup(Arg.Any<IEnvironmentService>(), Arg.Any<FileModel>(), Arg.Any<Environment>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenEnvironmentPopup_Fails_For_Null_Deserialization() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("null");

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, fileSystem, Substitute.For<IEnvironmentService>());

        await subject.OpenEnvironmentPopup(new(filePath));

        await popupService.DidNotReceive().ShowEnvironmentPopup(Arg.Any<IEnvironmentService>(), Arg.Any<FileModel>(), Arg.Any<Environment>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    // TODO can these tests be simplified since we use Reset method?
    [Fact]
    public async Task ResetPreferences_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(Models.ScriptEvaluationMode.Enabled);
        preferencesService.GetRequestTimeoutInSeconds().Returns(30);
        preferencesService.GetExchangeLoggingPath().Returns("./Path");
        preferencesService.GetExchangeLoggingOutputTemplate().Returns("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Foo.json"), new(@"C:\Documents\Bar.json"), new(@"C:\Documents\Bar.json")]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(@"C:\Documents\Foo.json"));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[Models.ScriptEvaluationMode.Disabled] },
            RequestTimeoutInSeconds = { Value = "300" },
            ExchangeLoggingPath = { Value = "./Location" },
            ExchangeLoggingOutputTemplate = { Value = "{Timestamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}" },
            Environments = { new(@"C:\Documents\New.json") },
            ActiveEnvironment = { Value = new(@"C:\Documents\New.json") }
        };

        await subject.ResetPreferences();

        Assert.Equal(preferencesService.GetMaximumRecentCollectionCount().ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[Models.ScriptEvaluationMode.Enabled], subject.ScriptEvaluationMode.Value);
        Assert.Equal(preferencesService.GetRequestTimeoutInSeconds().ToString(), subject.RequestTimeoutInSeconds.Value);
        Assert.Equal(preferencesService.GetExchangeLoggingPath(), subject.ExchangeLoggingPath.Value);
        Assert.Equal(preferencesService.GetExchangeLoggingOutputTemplate(), subject.ExchangeLoggingOutputTemplate.Value);
        Assert.Empty(subject.Environments);
        Assert.Null(subject.ActiveEnvironment.Value);

        preferencesService.Received(1).Reset();
        messageService.Received(1).Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task ResetPreferences_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(Models.ScriptEvaluationMode.Enabled);
        preferencesService.GetRequestTimeoutInSeconds().Returns(30);
        preferencesService.GetExchangeLoggingPath().Returns("./Path");
        preferencesService.GetExchangeLoggingOutputTemplate().Returns("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Foo.json"), new(@"C:\Documents\Bar.json")]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(@"C:\Documents\Foo.json"));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[Models.ScriptEvaluationMode.Disabled] },
            RequestTimeoutInSeconds = { Value = "300" },
            ExchangeLoggingPath = { Value = "./Location" },
            ExchangeLoggingOutputTemplate = { Value = "{Timestamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}" },
            Environments = { new(@"C:\Documents\New.json") },
            ActiveEnvironment = { Value = new(@"C:\Documents\New.json") }
        };

        await subject.ResetPreferences();

        Assert.Equal("25", subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[Models.ScriptEvaluationMode.Disabled], subject.ScriptEvaluationMode.Value);
        Assert.Equal("300", subject.RequestTimeoutInSeconds.Value);
        Assert.Equal("./Location", subject.ExchangeLoggingPath.Value);
        Assert.Equal("{Timestamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}", subject.ExchangeLoggingOutputTemplate.Value);
        Assert.Equal([new(@"C:\Documents\Bar.json"), new(@"C:\Documents\Foo.json"), new(@"C:\Documents\New.json")], subject.Environments);
        Assert.Equal(new(@"C:\Documents\New.json"), subject.ActiveEnvironment.Value);

        preferencesService.DidNotReceive().Reset();
        messageService.DidNotReceive().Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task ClearRecentCollections_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Environment.json")]);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        await subject.ClearRecentCollections();

        preferencesService.Received(1).ClearRecentCollections();
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task ClearRecentCollections_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Environment.json")]);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        await subject.ClearRecentCollections();

        preferencesService.DidNotReceive().ClearRecentCollections();
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
