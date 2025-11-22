using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class TextContentManagerTests {
    [Fact]
    public void Validate() {
        var subject = new TextContentManager();
        var content = new ContentTemplate() {
            StringContent = "Just a piece of text"
        };
        var collection = new RequestTemplateCollection();

        Assert.True(subject.Validate(content, collection));
    }

    [Fact]
    public void Format() {
        const string stringContent = "Just a piece of text";
        
        var subject = new TextContentManager();
        var content = new ContentTemplate() {
            StringContent = stringContent
        };
        var collection = new RequestTemplateCollection();

        Assert.True(subject.Format(content, collection));

        Assert.Equal(stringContent, content.StringContent);
    }

    [Fact]
    public async Task GetContent() {
        var subject = new TextContentManager();
        var content = new ContentTemplate() {
            StringContent = "The {{first}} and {{second}} get replaced"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "First", Value = "Replacement" },
                new() { Name =  "Second", Value = "Another" }
            }
        };

        var result = Assert.IsType<StringContent>(subject.GetContent(content, collection));

        Assert.Equal(JsonContentManager.DefaultMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("The Replacement and Another get replaced", await result.ReadAsStringAsync());
    }
}
