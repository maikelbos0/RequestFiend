using NSubstitute;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RequestFiend.Models.Tests;

public class PreferencesModelTests {
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(10, true)]
    public void Constructor(int maximumRecentCollectionCount, bool expectedSaveRecentCollections) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetMaximumRecentCollectionCount().Returns(maximumRecentCollectionCount);

        var subject = new PreferencesModel(preferencesService);

        Assert.Equal(maximumRecentCollectionCount, subject.MaximumRecentCollectionCount);
        Assert.Equal(expectedSaveRecentCollections, subject.SaveRecentCollections);
    }

    [Theory]
    [InlineData(10, false, 0)]
    [InlineData(0, true, 0)]
    [InlineData(10, true, 10)]
    public void Update(int maximumRecentCollectionCount, bool saveRecentCollections, int expectedMaximumRecentCollectionCount) {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetRecentCollections().Returns([.. Enumerable.Range(0, 11).Select(x => new RecentCollectionModel($"{x}.json"))]);

        var subject = new PreferencesModel(preferencesService) {
            MaximumRecentCollectionCount = maximumRecentCollectionCount,
            SaveRecentCollections = saveRecentCollections
        };

        subject.Update();

        preferencesService.Received().SetMaximumRecentCollectionCount(expectedMaximumRecentCollectionCount);
        preferencesService.Received().SetRecentCollections(Arg.Is<List<RecentCollectionModel>>(x => x.Count == expectedMaximumRecentCollectionCount));
    }

    [Fact]
    public void Reset() {
        var preferencesService = Substitute.For<IPreferencesService>();

        var subject = new PreferencesModel(preferencesService);

        subject.Reset();

        ;preferencesService.Received().Reset();
    }
}
