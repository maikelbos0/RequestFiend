using Xunit;

namespace RequestFiend.Core.Tests;

public class NoneContentManagerTests {
    [Fact]
    public void GetContent() {
        var subject = new NoneContentManager();
        var content = new ContentTemplate() {
            StringContent = "Just a piece of text"
        };
        var collection = new RequestTemplateCollection();

        Assert.Null(subject.GetContent(content, collection));
    }
}
