using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class JsonContentManagerTests {
    [Theory]
    [InlineData("", false)]
    [InlineData("\"Field\": \"Value\"", false)]
    [InlineData("{\"Field\": \"Value\"}", true)]
    [InlineData("[0, 1, 2, 3, 4, 5]", true)]
    public void Validate(string stringContent, bool expectedResult) {
        var subject = new JsonContentManager();
        var content = new ContentTemplate() {
            StringContent = stringContent
        };
        var collection = new RequestTemplateCollection();

        Assert.Equal(expectedResult, subject.Validate(content, collection));
    }

    [Fact]
    public void Format() {
        var subject = new JsonContentManager();
        var content = new ContentTemplate() {
            StringContent = "[{\"Field\":\"Value\"},{\"Field\":\"Value\"}]"
        };
        var collection = new RequestTemplateCollection();

        Assert.True(subject.Format(content, collection));

        Assert.Equal("[\r\n  {\r\n    \"Field\": \"Value\"\r\n  },\r\n  {\r\n    \"Field\": \"Value\"\r\n  }\r\n]", content.StringContent);
    }

    [Fact]
    public async Task GetContent() {
        var subject = new JsonContentManager();
        var content = new ContentTemplate() {
            StringContent = "[{{Node}}, {{Node}}]"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Node", Value = "{\"Meaning\": 42}" }
            }
        };

        var result = Assert.IsType<StringContent>(subject.GetContent(content, collection));

        Assert.Equal(JsonContentManager.DefaultMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("[{\"Meaning\": 42}, {\"Meaning\": 42}]", await result.ReadAsStringAsync());
    }
}
