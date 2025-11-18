using Xunit;

namespace RequestFiend.Core.Tests;

public class JsonContentTemplateTests {
    [Theory]
    [InlineData("", false)]
    [InlineData("\"Field\": \"Value\"", false)]
    [InlineData("{\"Field\": \"Value\"}", true)]
    [InlineData("[0, 1, 2, 3, 4, 5]", true)]
    public void Validate(string content, bool expectedResult) {
        var subject = new JsonContentTemplate() {
            Content = content
        };
        var collection = new RequestTemplateCollection();

        Assert.Equal(expectedResult, subject.Validate(collection));
    }

    [Fact]
    public void Format() {
        var subject = new JsonContentTemplate() {
            Content = "[{\"Field\":\"Value\"},{\"Field\":\"Value\"}]"
        };
        var collection = new RequestTemplateCollection();

        Assert.True(subject.Format(collection));

        Assert.Equal("[\r\n  {\r\n    \"Field\": \"Value\"\r\n  },\r\n  {\r\n    \"Field\": \"Value\"\r\n  }\r\n]", subject.Content);
    }

    [Fact]
    public void GetContent() {
        var subject = new JsonContentTemplate() {
            Content = "[{{Node}}, {{Node}}]"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Node", Value = "{\"Meaning\": 42}" }
            }
        };

        Assert.Equal("[{\"Meaning\": 42}, {\"Meaning\": 42}]"u8.ToArray(), subject.GetContent(collection));
    }
}
