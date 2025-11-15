using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionModelTests {
    [Fact]
    public void Constructor() {
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://default"
        };
        var subject = new RequestTemplateCollectionModel(collection);

        Assert.Equal(collection.DefaultUrl, subject.DefaultUrl.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("https://default")]
    public void TryCreateRequestTemplate(string? defaultUrl) {
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous"
        };
        var subject = new RequestTemplateCollectionModel(collection);

        subject.DefaultUrl.Value = defaultUrl;

        var result = subject.TryUpdateRequestTemplateCollection(collection);

        Assert.True(result);
        Assert.Equal(defaultUrl, collection.DefaultUrl);
    }
}
