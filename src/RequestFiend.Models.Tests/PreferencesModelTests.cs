using CommunityToolkit.Maui.Storage;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using Serilog.Events;
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
    public void LoggingPath() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            LoggingPath = { Value = "./Location" }
        };

        subject.LoggingPath.Set();

        preferencesService.Received().SetLoggingPath("./Location");
    }

    [Fact]
    public void LoggingOutputTemplate() {
        const string loggingOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            LoggingOutputTemplate = { Value = loggingOutputTemplate }
        };

        subject.LoggingOutputTemplate.Set();

        preferencesService.Received().SetLoggingOutputTemplate(loggingOutputTemplate);
    }

    [Fact]
    public void MinimumExchangeLoggingLevel() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            MinimumExchangeLoggingLevel = { Value = Options.LogEventLevelMap[LogEventLevel.Warning] }
        };

        subject.MinimumExchangeLoggingLevel.Set();

        preferencesService.Received().SetMinimumExchangeLoggingLevel(LogEventLevel.Warning);
    }

    [Fact]
    public void MinimumOtherSourceLoggingLevel() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            MinimumOtherSourceLoggingLevel = { Value = Options.LogEventLevelMap[LogEventLevel.Warning] }
        };

        subject.MinimumOtherSourceLoggingLevel.Set();

        preferencesService.Received().SetMinimumOtherSourceLoggingLevel(LogEventLevel.Warning);
    }

    [Fact]
    public void Environments() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            Environments = {
                new(@"C:\Documents\Environment.json")
            }
        };

        subject.Environments.Set();

        preferencesService.Received().SetEnvironments(subject.Environments);
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

    [Fact]
    public void Constructor() {
        const int maximumRecentCollectionCount = 10;
        const int requestTimeoutInSeconds = 90;
        const string loggingPath = "./Path";
        const string loggingOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        const string activeEnvironment = @"C:\Documents\Foo.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);
        preferencesService.GetScriptEvaluationMode().Returns(Models.ScriptEvaluationMode.CollectionScoped);
        preferencesService.GetRequestTimeoutInSeconds().Returns(requestTimeoutInSeconds);
        preferencesService.GetLoggingPath().Returns(loggingPath);
        preferencesService.GetLoggingOutputTemplate().Returns(loggingOutputTemplate);
        preferencesService.GetMinimumExchangeLoggingLevel().Returns(LogEventLevel.Warning);
        preferencesService.GetMinimumOtherSourceLoggingLevel().Returns(LogEventLevel.Error);
        preferencesService.GetEnvironments().Returns([new(activeEnvironment), new(@"C:\Documents\Bar.json"), new(@"C:\Documents\Bar.json")]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(activeEnvironment));

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>());

        Assert.Equal("Preferences", subject.PageTitleBase);
        Assert.Equal("Preferences", subject.ShellItemTitleBase);

        Assert.Equal(maximumRecentCollectionCount.ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[Models.ScriptEvaluationMode.CollectionScoped], subject.ScriptEvaluationMode.Value);
        Assert.Equal(requestTimeoutInSeconds.ToString(), subject.RequestTimeoutInSeconds.Value);
        Assert.Equal(loggingPath, subject.LoggingPath.Value);
        Assert.Equal(loggingOutputTemplate, subject.LoggingOutputTemplate.Value);
        Assert.Equal(Options.LogEventLevelMap[LogEventLevel.Warning], subject.MinimumExchangeLoggingLevel.Value);
        Assert.Equal(Options.LogEventLevelMap[LogEventLevel.Error], subject.MinimumOtherSourceLoggingLevel.Value);
        Assert.Equal(preferencesService.GetEnvironments().Distinct().OrderBy(environment => environment.Name, System.StringComparer.CurrentCultureIgnoreCase), subject.Environments);
        Assert.Equal(new(activeEnvironment), subject.ActiveEnvironment.Value);
        Assert.Contains(subject.Environments, environment => ReferenceEquals(environment, subject.ActiveEnvironment.Value));

        Assert.Equal([
            subject.RequestTimeoutInSeconds,
            subject.MaximumRecentCollectionCount,
            subject.ScriptEvaluationMode,
            subject.LoggingPath,
            subject.LoggingOutputTemplate,
            subject.MinimumExchangeLoggingLevel,
            subject.MinimumOtherSourceLoggingLevel,
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
    public async Task ShowEnvironmentPopup() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns(JsonSerializer.Serialize(new Environment()));
        var environmentService = Substitute.For<IEnvironmentService>();

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, fileSystem, environmentService);

        await subject.ShowEnvironmentPopup(new(filePath));

        await popupService.Received(1).ShowEnvironmentPopup(environmentService, new(filePath), Arg.Any<Environment>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task ShowEnvironmentPopup_Fails_For_Missing_File() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(false);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, fileSystem, Substitute.For<IEnvironmentService>());

        await subject.ShowEnvironmentPopup(new(filePath));

        await popupService.DidNotReceive().ShowEnvironmentPopup(Arg.Any<IEnvironmentService>(), Arg.Any<FileModel>(), Arg.Any<Environment>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task ShowEnvironmentPopup_Fails_For_Deserialization_Error() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("Invalid JSON");

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, fileSystem, Substitute.For<IEnvironmentService>());

        await subject.ShowEnvironmentPopup(new(filePath));

        await popupService.DidNotReceive().ShowEnvironmentPopup(Arg.Any<IEnvironmentService>(), Arg.Any<FileModel>(), Arg.Any<Environment>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task ShowEnvironmentPopup_Fails_For_Null_Deserialization() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("null");

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, fileSystem, Substitute.For<IEnvironmentService>());

        await subject.ShowEnvironmentPopup(new(filePath));

        await popupService.DidNotReceive().ShowEnvironmentPopup(Arg.Any<IEnvironmentService>(), Arg.Any<FileModel>(), Arg.Any<Environment>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task ResetPreferences_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            MaximumRecentCollectionCount = { Value = "25" }
        };

        await subject.ResetPreferences();

        Assert.False(subject.IsModified);

        preferencesService.Received(1).Reset();
        messageService.Received(1).Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task ResetPreferences_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>(), Substitute.For<IEnvironmentService>()) {
            MaximumRecentCollectionCount = { Value = "25" }
        };

        await subject.ResetPreferences();

        Assert.True(subject.IsModified);

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
