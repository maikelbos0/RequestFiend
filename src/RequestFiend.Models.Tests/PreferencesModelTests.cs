using NSubstitute;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
        const string environment = @"C:\Documents\Environment.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);
        preferencesService.GetScriptEvaluationMode().Returns(scriptEvaluationMode);
        preferencesService.GetRequestTimeoutInSeconds().Returns(requestTimeoutInSeconds);
        preferencesService.GetExchangeLoggingPath().Returns(exchangeLoggingPath);
        preferencesService.GetExchangeLoggingOutputTemplate().Returns(exchangeLoggingOutputTemplate);
        preferencesService.GetEnvironments().Returns([new(environment)]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(environment));

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>());

        Assert.Equal("Preferences", subject.PageTitleBase);
        Assert.Equal("Preferences", subject.ShellItemTitleBase);

        Assert.Equal(maximumRecentCollectionCount.ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[scriptEvaluationMode], subject.ScriptEvaluationMode.Value);
        Assert.Equal(requestTimeoutInSeconds.ToString(), subject.RequestTimeoutInSeconds.Value);
        Assert.Equal(exchangeLoggingPath, subject.ExchangeLoggingPath.Value);
        Assert.Equal(exchangeLoggingOutputTemplate, subject.ExchangeLoggingOutputTemplate.Value);
        Assert.Equal(new(environment), Assert.Single(subject.Environments));
        Assert.Equal(new(environment), subject.ActiveEnvironment.Value);

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
        const string existingEnvironment = @"C:\Documents\Environment.json";
        const string newEnvironment = @"C:\Documents\New.json";

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetEnvironments().Returns([new(existingEnvironment)]);
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
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

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
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
    public async Task Reset_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(ScriptEvaluationMode.Enabled);
        preferencesService.GetRequestTimeoutInSeconds().Returns(30);
        preferencesService.GetExchangeLoggingPath().Returns("./Path");
        preferencesService.GetExchangeLoggingOutputTemplate().Returns("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Environment.json")]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(@"C:\Documents\Environment.json"));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled] },
            RequestTimeoutInSeconds = { Value = "300" },
            ExchangeLoggingPath = { Value = "./Location" },
            ExchangeLoggingOutputTemplate = { Value = "{Timestamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}" },
            Environments = { new(@"C:\Documents\New.json") },
            ActiveEnvironment = { Value = new(@"C:\Documents\New.json") }
        };

        await subject.Reset();

        Assert.Equal(preferencesService.GetMaximumRecentCollectionCount().ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Enabled], subject.ScriptEvaluationMode.Value);
        Assert.Equal(preferencesService.GetRequestTimeoutInSeconds().ToString(), subject.RequestTimeoutInSeconds.Value);
        Assert.Equal(preferencesService.GetExchangeLoggingPath(), subject.ExchangeLoggingPath.Value);
        Assert.Equal(preferencesService.GetExchangeLoggingOutputTemplate(), subject.ExchangeLoggingOutputTemplate.Value);
        //Assert.Equal(preferencesService.GetEnvironments(), subject.Environments);
        Assert.Equal(preferencesService.GetActiveEnvironment(), subject.ActiveEnvironment.Value);

        preferencesService.Received(1).Reset();
        messageService.Received(1).Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(ScriptEvaluationMode.Enabled);
        preferencesService.GetRequestTimeoutInSeconds().Returns(30);
        preferencesService.GetExchangeLoggingPath().Returns("./Path");
        preferencesService.GetExchangeLoggingOutputTemplate().Returns("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        preferencesService.GetEnvironments().Returns([new(@"C:\Documents\Environment.json")]);
        preferencesService.GetActiveEnvironment().Returns(new FileModel(@"C:\Documents\Environment.json"));
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled] },
            RequestTimeoutInSeconds = { Value = "300" },
            ExchangeLoggingPath = { Value = "./Location" },
            ExchangeLoggingOutputTemplate = { Value = "{Timestamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}" },
            Environments = { new(@"C:\Documents\New.json") },
            ActiveEnvironment = { Value = new(@"C:\Documents\New.json") }
        };

        await subject.Reset();

        Assert.Equal("25", subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled], subject.ScriptEvaluationMode.Value);
        Assert.Equal("300", subject.RequestTimeoutInSeconds.Value);
        Assert.Equal("./Location", subject.ExchangeLoggingPath.Value);
        Assert.Equal("{Timestamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}", subject.ExchangeLoggingOutputTemplate.Value);
        Assert.Equal([new(@"C:\Documents\Environment.json"), new(@"C:\Documents\New.json")], subject.Environments);
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

        var subject = new PreferencesModel(preferencesService, messageService, popupService);

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

        var subject = new PreferencesModel(preferencesService, messageService, popupService);

        await subject.ClearRecentCollections();

        preferencesService.DidNotReceive().ClearRecentCollections();
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
