using NSubstitute;
using RequestFiend.Models.Services;
using System.Linq;
using Xunit;

namespace RequestFiend.Models.Tests;

public class PreferencesModelTests {
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(10, true)]
    public void Constructor(int maximumRecentCollectionCount, bool saveRecentCollections) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetSaveRecentCollections().Returns(saveRecentCollections);
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);

        var subject = new PreferencesModel(preferencesService);

        Assert.Equal(saveRecentCollections, subject.SaveRecentCollections);
        Assert.Equal(maximumRecentCollectionCount, subject.MaximumRecentCollectionCount);
    }

    [Theory]
    [InlineData(10, false)]
    [InlineData(0, true)]
    [InlineData(10, true)]
    public void Update(int maximumRecentCollectionCount, bool saveRecentCollections) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetRecentCollections().Returns([.. Enumerable.Range(0, 11).Select(x => new RecentCollectionModel($"{x}.json"))]);

        var subject = new PreferencesModel(preferencesService) {
            MaximumRecentCollectionCount = maximumRecentCollectionCount,
            SaveRecentCollections = saveRecentCollections
        };

        subject.Update();

        preferencesService.Received().SetSaveRecentCollections(saveRecentCollections);
        preferencesService.Received().SetMaximumRecentCollectionCount(maximumRecentCollectionCount);

        if (saveRecentCollections) {
            preferencesService.Received().TrimRecentCollections();
        }
        else {
            preferencesService.Received().ClearRecentCollections();
        }
    }

    [Fact]
    public void Reset() {
        var preferencesService = Substitute.For<IPreferencesService>();

        var subject = new PreferencesModel(preferencesService);

        subject.Reset();

        ;preferencesService.Received().Reset();
    }
}
