using NSubstitute;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.Linq;
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

        Assert.Equal(saveRecentCollections, subject.ShowRecentCollections);
        Assert.Equal(maximumRecentCollectionCount, subject.MaximumRecentCollectionCount);
    }

    [Theory]
    [InlineData(10, false)]
    [InlineData(0, true)]
    [InlineData(10, true)]
    public void Update(int maximumRecentCollectionCount, bool saveRecentCollections) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetRecentCollections().Returns([.. Enumerable.Range(0, 11).Select(x => new RecentCollectionModel($"{x}.json"))]);
        var messageService = Substitute.For<IMessageService>();

        var subject = new PreferencesModel(preferencesService, messageService, Substitute.For<IPopupService>()) {
            MaximumRecentCollectionCount = maximumRecentCollectionCount,
            ShowRecentCollections = saveRecentCollections
        };

        subject.Update();

        preferencesService.Received().SetShowRecentCollections(saveRecentCollections);
        preferencesService.Received().SetMaximumRecentCollectionCount(maximumRecentCollectionCount);

        if (saveRecentCollections) {
            preferencesService.Received().TrimRecentCollections();
        }
        else {
            preferencesService.Received().ClearRecentCollections();
        }

        messageService.Received().Send(Arg.Any<SuccessMessage>());
    [Fact]
    public async Task Reset_And_Confirm() {
        var preferencesService = Substitute.For<IPreferencesService>();
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);

        var subject = new PreferencesModel(preferencesService, messageService, popupService);

        await subject.Reset();

        preferencesService.Received(1).Reset();
        preferencesService.Received(2).GetShowRecentCollections();
        preferencesService.Received(2).GetMaximumRecentCollectionCount();
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Reset_Without_Confirming() {
        var preferencesService = Substitute.For<IPreferencesService>();
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);

        var subject = new PreferencesModel(preferencesService, messageService, popupService);

        await subject.Reset();

        preferencesService.DidNotReceive().Reset();
        preferencesService.Received(1).GetShowRecentCollections();
        preferencesService.Received(1).GetMaximumRecentCollectionCount();
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
