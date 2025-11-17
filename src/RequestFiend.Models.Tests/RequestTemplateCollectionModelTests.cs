using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionModelTests {
    [Fact]
    public void Constructor() {
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://default",
            DefaultHeaders = {
                new() { Name = "Accept", Value = "application/json" },
                new() { Name = "X-api-key", Value = "4p1-k3y" }
            }
        };
        var subject = new RequestTemplateCollectionModel(collection);

        Assert.Equal(collection.DefaultUrl, subject.DefaultUrl.Value);
        Assert.Equal(collection.DefaultHeaders.Count, subject.DefaultHeaders.Count);

        foreach (var header in collection.DefaultHeaders) {
            Assert.Equal(header.Value, Assert.Single(subject.DefaultHeaders, headerModel => headerModel.Name == header.Name).Value);
        }
    }

    [Fact]
    public void TryUpdateRequestTemplateCollection() {
        const string defaultUrl = "https://default";
        const string headerName = "Name";
        const string headerValue = "Value";

        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            DefaultHeaders = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };
        var subject = new RequestTemplateCollectionModel(collection);

        subject.DefaultUrl.Value = defaultUrl;
        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.DefaultHeaders[0].Value.Value = headerValue;

        var result = subject.TryUpdateRequestTemplateCollection(collection);

        Assert.True(result);
        Assert.Equal(defaultUrl, collection.DefaultUrl);
        Assert.Equal(headerName, collection.DefaultHeaders[0].Name);
        Assert.Equal(headerValue, collection.DefaultHeaders[0].Value);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("Name", null)]
    [InlineData(null, "Value")]
    public void TryUpdateRequestTemplateCollection_Fails_When_Invalid(string? headerName, string? headerValue) {
        const string defaultUrl = "https://default";
        
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            DefaultHeaders = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };
        var subject = new RequestTemplateCollectionModel(collection);

        subject.DefaultUrl.Value = defaultUrl;
        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.DefaultHeaders[0].Value.Value = headerValue;

        var result = subject.TryUpdateRequestTemplateCollection(collection);

        Assert.False(result);
        Assert.NotEqual(defaultUrl, collection.DefaultUrl);
        Assert.NotEqual(headerName, collection.DefaultHeaders[0].Name);
        Assert.NotEqual(headerValue, collection.DefaultHeaders[0].Value);
    }
}
