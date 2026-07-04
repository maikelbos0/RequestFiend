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
    [InlineData(ScriptEvaluationMode.Disabled)]
    [InlineData(ScriptEvaluationMode.Enabled)]
    [InlineData(ScriptEvaluationMode.CollectionScoped)]
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

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>());

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

    [Theory]
    [InlineData("", 0, "10", 10)]
    [InlineData("0", 0, "1", 1)]
    [InlineData("1", 1, "0", 0)]
    [InlineData("10", 10, "", null)]
    public void Update(string maximumRecentCollectionCount, int expectedMaximumRecentCollectionCount, string requestTimeoutInSeconds, int? expectedRequestTimeoutInSeconds) {
        const ScriptEvaluationMode scriptEvaluationMode = ScriptEvaluationMode.Enabled;
        const string exchangeLoggingPath = "./Path";
        const string exchangeLoggingOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        const string existingEnvironment = @"C:\Documents\Foo.json";
        const string newEnvironment = @"C:\Documents\Bar.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(existingEnvironment)]);
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>(), Substitute.For<IFileSystem>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount.ToString() },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[scriptEvaluationMode] },
            RequestTimeoutInSeconds = { Value = requestTimeoutInSeconds },
            ExchangeLoggingPath = { Value = exchangeLoggingPath },
            ExchangeLoggingOutputTemplate = { Value = exchangeLoggingOutputTemplate },
            Environments = { new(newEnvironment) },
            ActiveEnvironment = { Value = new(existingEnvironment) }
        };

        subject.Update();

        preferencesService.Received(1).SetMaximumRecentCollectionCount(expectedMaximumRecentCollectionCount);
        preferencesService.Received(1).SetScriptEvaluationMode(scriptEvaluationMode);
        preferencesService.Received(1).SetRequestTimeoutInSeconds(expectedRequestTimeoutInSeconds);
        preferencesService.Received(1).SetExchangeLoggingPath(exchangeLoggingPath);
        preferencesService.Received(1).SetExchangeLoggingOutputTemplate(exchangeLoggingOutputTemplate);
        preferencesService.Received(1).SetEnvironments(Arg.Is<FileModelCollection>(collection => collection.SequenceEqual(new FileModel[] { new(existingEnvironment), new(newEnvironment) })));
        preferencesService.Received(1).SetActiveEnvironment(new(existingEnvironment));

        Assert.False(subject.MaximumRecentCollectionCount.IsModified);
        Assert.False(subject.ScriptEvaluationMode.IsModified);
        Assert.False(subject.RequestTimeoutInSeconds.IsModified);
        Assert.False(subject.ExchangeLoggingPath.IsModified);
        Assert.False(subject.ExchangeLoggingOutputTemplate.IsModified);
        Assert.False(subject.Environments.IsModified);
        Assert.False(subject.ActiveEnvironment.IsModified);

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

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>(), Substitute.For<IFileSystem>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount },
            RequestTimeoutInSeconds = { Value = requestTimeoutInSeconds }
        };

        subject.Update();

        preferencesService.DidNotReceive().SetMaximumRecentCollectionCount(Arg.Any<int>());
        preferencesService.DidNotReceive().SetScriptEvaluationMode(Arg.Any<ScriptEvaluationMode>());
        preferencesService.DidNotReceive().SetRequestTimeoutInSeconds(Arg.Any<int?>());
        preferencesService.DidNotReceive().SetExchangeLoggingPath(Arg.Any<string>());
        preferencesService.DidNotReceive().SetExchangeLoggingOutputTemplate(Arg.Any<string>());
        preferencesService.DidNotReceive().SetEnvironments(Arg.Any<IEnumerable<FileModel>>());
        preferencesService.DidNotReceive().SetActiveEnvironment(Arg.Any<FileModel?>());

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

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>());

        await subject.CreateNewEnvironment();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new Environment())));
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
        messageService.Received(1).Send(Arg.Is<OpenEnvironmentMessage>(message => message.File == new FileModel(newEnvironment)));
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

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>());

        await subject.CreateNewEnvironment();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new Environment())));
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
        messageService.Received(1).Send(Arg.Is<OpenEnvironmentMessage>(message => message.File == new FileModel(newEnvironment)));
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

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>());

        await subject.CreateNewEnvironment();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new Environment())));
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        messageService.DidNotReceive().Send(Arg.Any<OpenEnvironmentMessage>());
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

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>());

        await subject.CreateNewEnvironment();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new Environment())));
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        messageService.DidNotReceive().Send(Arg.Any<OpenEnvironmentMessage>());
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

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, Substitute.For<IFileSystem>());

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

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, Substitute.For<IFileSystem>());

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

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), popupService, Substitute.For<IFileSystem>());

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

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>());

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

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), Substitute.For<IFileSystem>());

        subject.RemoveEnvironment(new(activeEnvironment));

        Assert.Equal([new(otherEnvironment)], subject.Environments);
        Assert.Null(subject.ActiveEnvironment.Value);
    }

    [Fact]
    public async Task OpenEnvironmentPopup() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns(JsonSerializer.Serialize(new Environment()));

        var subject = new PreferencesModel(preferencesService, messageService, popupService, fileSystem);

        await subject.OpenEnvironmentPopup(new(filePath));

        messageService.Received(1).Send(Arg.Is<OpenEnvironmentMessage>(message => message.File == new FileModel(filePath)));
        messageService.Received(1).Send(Arg.Is<OpenEnvironmentMessage>(message => true));
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenEnvironmentPopup_Fails_For_Missing_File() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, fileSystem);

        await subject.OpenEnvironmentPopup(new(filePath));

        messageService.DidNotReceive().Send(Arg.Any<OpenEnvironmentMessage>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenEnvironmentPopup_Fails_For_Deserialization_Error() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("Invalid JSON");

        var subject = new PreferencesModel(preferencesService, messageService, popupService, fileSystem);

        await subject.OpenEnvironmentPopup(new(filePath));

        messageService.DidNotReceive().Send(Arg.Any<OpenEnvironmentMessage>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenEnvironmentPopup_Fails_For_Null_Deserialization() {
        const string filePath = @"C:\Documents\Local.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([]);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("null");

        var subject = new PreferencesModel(preferencesService, messageService, popupService, fileSystem);

        await subject.OpenEnvironmentPopup(new(filePath));

        messageService.DidNotReceive().Send(Arg.Any<OpenEnvironmentMessage>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task ResetPreferences_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(ScriptEvaluationMode.Enabled);
        preferencesService.GetRequestTimeoutInSeconds().Returns(30);
        preferencesService.GetExchangeLoggingPath().Returns("./Path");
        preferencesService.GetExchangeLoggingOutputTemplate().Returns("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Foo.json"), new(@"C:\Documents\Bar.json"), new(@"C:\Documents\Bar.json")]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(@"C:\Documents\Foo.json"));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>()) {
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled] },
            RequestTimeoutInSeconds = { Value = "300" },
            ExchangeLoggingPath = { Value = "./Location" },
            ExchangeLoggingOutputTemplate = { Value = "{Timestamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}" },
            Environments = { new(@"C:\Documents\New.json") },
            ActiveEnvironment = { Value = new(@"C:\Documents\New.json") }
        };

        await subject.ResetPreferences();

        Assert.Equal(preferencesService.GetMaximumRecentCollectionCount().ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Enabled], subject.ScriptEvaluationMode.Value);
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
        preferencesService.GetScriptEvaluationMode().Returns(ScriptEvaluationMode.Enabled);
        preferencesService.GetRequestTimeoutInSeconds().Returns(30);
        preferencesService.GetExchangeLoggingPath().Returns("./Path");
        preferencesService.GetExchangeLoggingOutputTemplate().Returns("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Foo.json"), new(@"C:\Documents\Bar.json")]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(@"C:\Documents\Foo.json"));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>()) {
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled] },
            RequestTimeoutInSeconds = { Value = "300" },
            ExchangeLoggingPath = { Value = "./Location" },
            ExchangeLoggingOutputTemplate = { Value = "{Timestamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}" },
            Environments = { new(@"C:\Documents\New.json") },
            ActiveEnvironment = { Value = new(@"C:\Documents\New.json") }
        };

        await subject.ResetPreferences();

        Assert.Equal("25", subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled], subject.ScriptEvaluationMode.Value);
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

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>());

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

        var subject = new PreferencesModel(preferencesService, messageService, popupService, Substitute.For<IFileSystem>());

        await subject.ClearRecentCollections();

        preferencesService.DidNotReceive().ClearRecentCollections();
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
