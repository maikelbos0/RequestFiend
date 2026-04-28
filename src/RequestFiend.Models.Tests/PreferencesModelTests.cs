using NSubstitute;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class PreferencesModelTests {
    [Theory]
    [InlineData(0, false, ScriptEvaluationMode.Disabled)]
    [InlineData(1, true, ScriptEvaluationMode.Enabled)]
    [InlineData(10, true, ScriptEvaluationMode.CollectionScoped)]
    public void Constructor(int maximumRecentCollectionCount, bool saveRecentCollections, ScriptEvaluationMode scriptEvaluationMode) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(saveRecentCollections);
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);
        preferencesService.GetScriptEvaluationMode().Returns(scriptEvaluationMode);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>());

        Assert.Equal("Preferences", subject.PageTitleBase);
        Assert.Equal("Preferences", subject.ShellItemTitleBase);

        Assert.Equal(saveRecentCollections, subject.ShowRecentCollections.Value);
        Assert.Equal(maximumRecentCollectionCount.ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[scriptEvaluationMode], subject.ScriptEvaluationMode.Value);

        Assert.Equal([subject.ShowRecentCollections, subject.MaximumRecentCollectionCount, subject.ScriptEvaluationMode], subject.Validatables);
    }

    [Theory]
    [InlineData(10, false)]
    [InlineData(0, true)]
    [InlineData(10, true)]
    public void Update(int maximumRecentCollectionCount, bool showRecentCollections) {
        const ScriptEvaluationMode scriptEvaluationMode = ScriptEvaluationMode.Enabled;

        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(showRecentCollections);
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount.ToString() },
            ShowRecentCollections = { Value = showRecentCollections },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[scriptEvaluationMode] }
        };

        subject.Update();

        preferencesService.Received(1).SetShowRecentCollections(showRecentCollections);
        preferencesService.Received(1).SetMaximumRecentCollectionCount(maximumRecentCollectionCount);
        preferencesService.Received(1).SetScriptEvaluationMode(scriptEvaluationMode);

        Assert.False(subject.ShowRecentCollections.IsModified);
        Assert.False(subject.MaximumRecentCollectionCount.IsModified);
        Assert.False(subject.ScriptEvaluationMode.IsModified);

        if (showRecentCollections) {
            preferencesService.Received(1).TrimRecentCollections();
        }
        else {
            preferencesService.Received(1).ClearRecentCollections();
        }

        messageService.Received(1).Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Invalid")]
    public void Update_Fails_When_Invalid(string maximumRecentCollectionCount) {
        var preferencesService = Substitute.For<IPreferencesService>();
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount },
        };

        subject.Update();

        preferencesService.DidNotReceive().SetShowRecentCollections(Arg.Any<bool>());
        preferencesService.DidNotReceive().SetMaximumRecentCollectionCount(Arg.Any<int>());
        preferencesService.DidNotReceive().SetScriptEvaluationMode(Arg.Any<ScriptEvaluationMode>());

        preferencesService.DidNotReceive().TrimRecentCollections();
        preferencesService.DidNotReceive().ClearRecentCollections();

        messageService.DidNotReceive().Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(true);
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(ScriptEvaluationMode.Enabled);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            ShowRecentCollections = { Value = false },
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled] }
        };

        await subject.Reset();

        Assert.Equal(preferencesService.GetShowRecentCollections(), subject.ShowRecentCollections.Value);
        Assert.Equal(preferencesService.GetMaximumRecentCollectionCount().ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Enabled], subject.ScriptEvaluationMode.Value);

        preferencesService.Received(1).Reset();
        messageService.Received(1).Send(Arg.Any<PreferencesUpdatedMessage>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(true);
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetScriptEvaluationMode().Returns(ScriptEvaluationMode.Enabled);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            ShowRecentCollections = { Value = false },
            MaximumRecentCollectionCount = { Value = "25" },
            ScriptEvaluationMode = { Value = Options.ScriptEvaluationModeMap[ScriptEvaluationMode.Disabled] }
        };

        await subject.Reset();

        Assert.False(subject.ShowRecentCollections.Value);
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

    [Fact]
    public void ToggleShowRecentCollections() {
        var subject = new PreferencesModel(Substitute.For<IPreferencesService>(), Substitute.For<IMessageService>(), Substitute.For<IPopupService>()) {
            ShowRecentCollections = { Value = true }
        };

        subject.ToggleShowRecentCollections();

        Assert.False(subject.ShowRecentCollections.Value);
    }
}
