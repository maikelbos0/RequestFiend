using NSubstitute;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class PreferencesModelTests {
    [Theory]
    [InlineData(0, false, true)]
    [InlineData(1, true, false)]
    [InlineData(10, true, true)]
    public void Constructor(int maximumRecentCollectionCount, bool saveRecentCollections, bool allowScriptExecution) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(saveRecentCollections);
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);
        preferencesService.GetAllowScriptExecution().Returns(allowScriptExecution);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>());

        Assert.Equal("Preferences", subject.PageTitleBase);
        Assert.Equal("Preferences", subject.ShellItemTitleBase);
        Assert.Equal(saveRecentCollections, subject.ShowRecentCollections.Value);
        Assert.Equal(maximumRecentCollectionCount.ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(allowScriptExecution, subject.AllowScriptExecution.Value);
    }

    [Theory]
    [InlineData(10, false, true)]
    [InlineData(0, true, false)]
    [InlineData(10, true, true)]
    public void Update(int maximumRecentCollectionCount, bool showRecentCollections, bool allowScriptExecution) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(showRecentCollections);
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount.ToString() },
            ShowRecentCollections = { Value = showRecentCollections },
            AllowScriptExecution = { Value = allowScriptExecution }
        };

        subject.Update();

        preferencesService.Received(1).SetShowRecentCollections(showRecentCollections);
        preferencesService.Received(1).SetMaximumRecentCollectionCount(maximumRecentCollectionCount);
        preferencesService.Received(1).SetAllowScriptExecution(allowScriptExecution);

        Assert.False(subject.ShowRecentCollections.IsModified);
        Assert.False(subject.MaximumRecentCollectionCount.IsModified);
        Assert.False(subject.AllowScriptExecution.IsModified);

        if (showRecentCollections) {
            preferencesService.Received(1).TrimRecentCollections();
        }
        else {
            preferencesService.Received(1).ClearRecentCollections();
        }

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
        preferencesService.DidNotReceive().SetAllowScriptExecution(Arg.Any<bool>());

        preferencesService.DidNotReceive().TrimRecentCollections();
        preferencesService.DidNotReceive().ClearRecentCollections();

        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(true);
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetAllowScriptExecution().Returns(true);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            ShowRecentCollections = { Value = false },
            MaximumRecentCollectionCount = { Value = "25" },
            AllowScriptExecution = { Value = false }
        };

        await subject.Reset();

        Assert.Equal(preferencesService.GetShowRecentCollections(), subject.ShowRecentCollections.Value);
        Assert.Equal(preferencesService.GetMaximumRecentCollectionCount().ToString(), subject.MaximumRecentCollectionCount.Value);
        Assert.Equal(preferencesService.GetAllowScriptExecution(), subject.AllowScriptExecution.Value);

        preferencesService.Received(1).Reset();
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(true);
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        preferencesService.GetAllowScriptExecution().Returns(true);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            ShowRecentCollections = { Value = false },
            MaximumRecentCollectionCount = { Value = "25" },
            AllowScriptExecution = { Value = false }
        };

        await subject.Reset();

        Assert.False(subject.ShowRecentCollections.Value);
        Assert.Equal("25", subject.MaximumRecentCollectionCount.Value);
        Assert.False(subject.AllowScriptExecution.Value);

        preferencesService.DidNotReceive().Reset();
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

    [Fact]
    public void ToggleAllowScriptExecution() {
        var subject = new PreferencesModel(Substitute.For<IPreferencesService>(), Substitute.For<IMessageService>(), Substitute.For<IPopupService>()) {
            AllowScriptExecution = { Value = true }
        };

        subject.ToggleAllowScriptExecution();

        Assert.False(subject.AllowScriptExecution.Value);
    }
}
