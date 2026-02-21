using NSubstitute;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class PreferencesModelTests {
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(10, true)]
    public void Constructor(int maximumRecentCollectionCount, bool saveRecentCollections) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(saveRecentCollections);
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);

        var subject = new PreferencesModel(preferencesService, Substitute.For<IMessageService>(), Substitute.For<IPopupService>());

        Assert.Equal("Preferences", subject.PageTitleBase);
        Assert.Equal("Preferences", subject.ShellItemTitleBase);
        Assert.Equal(saveRecentCollections, subject.ShowRecentCollections.Value);
        Assert.Equal(maximumRecentCollectionCount.ToString(), subject.MaximumRecentCollectionCount.Value);
    }

    [Theory]
    [InlineData(10, false)]
    [InlineData(0, true)]
    [InlineData(10, true)]
    public void Update(int maximumRecentCollectionCount, bool showRecentCollections) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(showRecentCollections);
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
            MaximumRecentCollectionCount = { Value = maximumRecentCollectionCount.ToString() },
            ShowRecentCollections = { Value = showRecentCollections }
        };

        subject.Update();

        preferencesService.Received(1).SetShowRecentCollections(showRecentCollections);
        preferencesService.Received(1).SetMaximumRecentCollectionCount(maximumRecentCollectionCount);

        Assert.False(subject.ShowRecentCollections.IsModified);
        Assert.False(subject.MaximumRecentCollectionCount.IsModified);

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

        preferencesService.DidNotReceive().TrimRecentCollections();
        preferencesService.DidNotReceive().ClearRecentCollections();

        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(true);
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            ShowRecentCollections = { Value = false },
            MaximumRecentCollectionCount = { Value = "25" }
        };

        await subject.Reset();

        Assert.Equal(preferencesService.GetShowRecentCollections(), subject.ShowRecentCollections.Value);
        Assert.Equal(preferencesService.GetMaximumRecentCollectionCount().ToString(), subject.MaximumRecentCollectionCount.Value);

        preferencesService.Received(1).Reset();
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(true);
        preferencesService.GetMaximumRecentCollectionCount().Returns(10);
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService) {
            ShowRecentCollections = { Value = false },
            MaximumRecentCollectionCount = { Value = "25" }
        };

        await subject.Reset();

        Assert.False(subject.ShowRecentCollections.Value);
        Assert.Equal("25", subject.MaximumRecentCollectionCount.Value);

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
}
