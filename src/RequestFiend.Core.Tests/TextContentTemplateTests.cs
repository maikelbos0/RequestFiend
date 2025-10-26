using Xunit;

namespace RequestFiend.Core.Tests;

public class TextContentTemplateTests {
    [Fact]
    public void Validate() {
        var subject = new TextContentTemplate() {
            Text = "Just a piece of text"
        };
        var collection = new RequestTemplateCollection() {
            Name = "Collection"
        };

        Assert.True(subject.Validate(collection));
    }

    [Fact]
    public void Format() {
        const string text = "Just a piece of text";
        var subject = new TextContentTemplate() {
            Text = text
        };
        var collection = new RequestTemplateCollection() {
            Name = "Collection"
        };

        subject.Format(collection);

        Assert.Equal(text, subject.Text);
    }

    [Fact]
    public void GetContent() {
        var subject = new TextContentTemplate() {
            Text = "The {{first}} and {{second}} get replaced"
        };
        var collection = new RequestTemplateCollection() {
            Name = "Collection",
            Variables = {
                { "First", "Replacement" },
                { "Second", "Another" }
            }
        };

        Assert.Equal("The Replacement and Another get replaced"u8.ToArray(), subject.GetContent(collection));
    }
}
