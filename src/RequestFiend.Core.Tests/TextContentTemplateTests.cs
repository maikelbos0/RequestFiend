using Xunit;

namespace RequestFiend.Core.Tests;

public class TextContentTemplateTests {
    [Fact]
    public void Validate() {
        var subject = new TextContentTemplate() {
            Content = "Just a piece of text"
        };
        var collection = new RequestTemplateCollection();

        Assert.True(subject.Validate(collection));
    }

    [Fact]
    public void Format() {
        const string content = "Just a piece of text";
        var subject = new TextContentTemplate() {
            Content = content
        };
        var collection = new RequestTemplateCollection();

        Assert.True(subject.Format(collection));

        Assert.Equal(content, subject.Content);
    }

    [Fact]
    public void GetContent() {
        var subject = new TextContentTemplate() {
            Content = "The {{first}} and {{second}} get replaced"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                { "First", "Replacement" },
                { "Second", "Another" }
            }
        };

        Assert.Equal("The Replacement and Another get replaced"u8.ToArray(), subject.GetContent(collection));
    }
}
