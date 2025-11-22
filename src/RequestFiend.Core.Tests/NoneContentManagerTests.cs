using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class NoneContentManagerTests {
    [Fact]
    public void Validate() {
        var subject = new NoneContentManager();
        var content = new ContentTemplate() {
            StringContent = "Just a piece of text"
        };
        var collection = new RequestTemplateCollection();

        Assert.True(subject.Validate(content, collection));
    }

    [Fact]
    public void Format() {
        const string stringContent = "Just a piece of text";

        var subject = new NoneContentManager();
        var content = new ContentTemplate() {
            StringContent = stringContent
        };
        var collection = new RequestTemplateCollection();

        Assert.True(subject.Format(content, collection));

        Assert.Equal(stringContent, content.StringContent);
    }

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
