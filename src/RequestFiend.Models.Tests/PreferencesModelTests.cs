using NSubstitute;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
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
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);
        preferencesService.GetScriptEvaluationMode().Returns(scriptEvaluationMode);
        preferencesService.GetRequestTimeoutInSeconds().Returns(requestTimeoutInSeconds);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>());

        Assert.Equal("Preferences", subject.PageTitleBase);
        Assert.Equal("Preferences", subject.ShellItemTitleBase);

        Assert.Equal(maximumRecentCollectionCount.ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[scriptEvaluationMode], subject.ScriptEvaluationMode.Value);
        Assert.Equal(requestTimeoutInSeconds.ToString(), subject.RequestTimeoutInSeconds.Value);

        Assert.Equal([subject.RequestTimeoutInSeconds, subject.MaximumRecentCollectionCount, subject.ScriptEvaluationMode], subject.Validatables);
    }

    [Theory]
    [InlineData("", 0, "10", 10)]
    [InlineData("0", 0, "1", 1)]
    [InlineData("1", 1, "0", 0)]
    [InlineData("10", 10, "", null)]
    public void Update(string maximumRecentCollectionCount, int expectedMaximumRecentCollectionCount, string requestTimeoutInSeconds, int? expectedRequestTimeoutInSeconds) {
        const ScriptEvaluationMode scriptEvaluationMode = ScriptEvaluationMode.Enabled;

        var preferencesService = Substitute.For<IPreferencesService>();
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount.ToString() },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[scriptEvaluationMode] },
            RequestTimeoutInSeconds = { Value = requestTimeoutInSeconds }
        };

        subject.Update();

        preferencesService.Received(1).SetMaximumRecentCollectionCount(expectedMaximumRecentCollectionCount);
        preferencesService.Received(1).SetScriptEvaluationMode(scriptEvaluationMode);
        preferencesService.Received(1).SetRequestTimeoutInSeconds(expectedRequestTimeoutInSeconds);

        Assert.False(subject.MaximumRecentCollectionCount.IsModified);
        Assert.False(subject.ScriptEvaluationMode.IsModified);
        Assert.False(subject.RequestTimeoutInSeconds.IsModified);

        preferencesService.Received(1).TrimRecentCollections();

        messageService.Received(1).Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("Invalid", "10")]
    [InlineData("10", "Invalid")]
    public void Update_Fails_When_Invalid(string maximumRecentCollectionCount,string requestTimeoutInSeconds) {
        var preferencesService = Substitute.For<IPreferencesService>();
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount },
            RequestTimeoutInSeconds = { Value = requestTimeoutInSeconds }
        };

        subject.Update();

        preferencesService.DidNotReceive().SetMaximumRecentCollectionCount(Arg.Any<int>());
        preferencesService.DidNotReceive().SetScriptEvaluationMode(Arg.Any<ScriptEvaluationMode>());
        preferencesService.DidNotReceive().SetRequestTimeoutInSeconds(Arg.Any<int?>());

        preferencesService.DidNotReceive().TrimRecentCollections();

        messageService.DidNotReceive().Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(ScriptEvaluationMode.Enabled);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled] }
        };

        await subject.Reset();

        Assert.Equal(preferencesService.GetMaximumRecentCollectionCount().ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Enabled], subject.ScriptEvaluationMode.Value);

        preferencesService.Received(1).Reset();
        messageService.Received(1).Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(ScriptEvaluationMode.Enabled);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled] }
        };

        await subject.Reset();

        Assert.Equal("25", subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled], subject.ScriptEvaluationMode.Value);

        preferencesService.DidNotReceive().Reset();
        messageService.DidNotReceive().Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task ClearRecentCollections_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
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
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService);

        await subject.ClearRecentCollections();

        preferencesService.DidNotReceive().ClearRecentCollections();
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
