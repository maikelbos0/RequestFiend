using Xunit;

namespace RequestFiend.Models.Tests;

public class RecentCollectionModelTests {
    [Fact]
    public void Name() {
        var subject = new RecentCollectionModel(@"C:\Path\To\File.json");

        Assert.Equal("File", subject.Name);
    }
}
