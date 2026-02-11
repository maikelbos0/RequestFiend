using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionFileModelTests {
    [Fact]
    public void Name() {
        var subject = new RequestTemplateCollectionFileModel(@"C:\Path\To\File.json");

        Assert.Equal("File", subject.Name);
    }
}
