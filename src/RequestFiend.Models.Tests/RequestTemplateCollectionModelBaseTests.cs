using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionModelBaseTests {
    [Fact]
    public void Constructor() {
        var subject = new RequestTemplateCollectionModelBase(@"C:\Documents\External data requests.json", new());

        Assert.Equal("External data requests", subject.Title);
    }
}
