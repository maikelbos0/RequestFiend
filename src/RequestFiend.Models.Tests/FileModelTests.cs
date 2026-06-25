using Xunit;

namespace RequestFiend.Models.Tests;

public class FileModelTests {
    [Fact]
    public void Name() {
        var subject = new FileModel(@"C:\Path\To\File.json");

        Assert.Equal("File", subject.Name);
    }
}
