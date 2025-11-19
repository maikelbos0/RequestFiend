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
        const string variableName = "Name";
        const string variableValue = "Value";

        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            DefaultHeaders = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            Variables = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };
        var subject = new RequestTemplateCollectionModel(collection);

        subject.DefaultUrl.Value = defaultUrl;
        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.DefaultHeaders[0].Value.Value = headerValue;
        subject.Variables[0].Name.Value = variableName;
        subject.Variables[0].Value.Value = variableValue;

        var result = subject.TryUpdateRequestTemplateCollection(collection);

        Assert.True(result);
        Assert.Equal(defaultUrl, collection.DefaultUrl);
        Assert.Equal(headerName, collection.DefaultHeaders[0].Name);
        Assert.Equal(headerValue, collection.DefaultHeaders[0].Value);
        Assert.Equal(variableName, collection.Variables[0].Name);
        Assert.Equal(variableValue, collection.Variables[0].Value);
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData("Name", null, "Name", "Value")]
    [InlineData(null, "Value", "Name", "Value")]
    [InlineData("Name", "Value", null, "Value")]
    [InlineData("Name", "Value", "Name", null)]
    public void TryUpdateRequestTemplateCollection_Fails_When_Invalid(string? headerName, string? headerValue, string? variableName, string? variableValue) {
        const string defaultUrl = "https://default";

        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://previous",
            DefaultHeaders = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            Variables = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };
        var subject = new RequestTemplateCollectionModel(collection);

        subject.DefaultUrl.Value = defaultUrl;
        subject.DefaultHeaders[0].Name.Value = headerName;
        subject.DefaultHeaders[0].Value.Value = headerValue;
        subject.Variables[0].Name.Value = variableName;
        subject.Variables[0].Value.Value = variableValue;

        var result = subject.TryUpdateRequestTemplateCollection(collection);

        Assert.False(result);
        Assert.NotEqual(defaultUrl, collection.DefaultUrl);
        Assert.NotEqual(headerName, collection.DefaultHeaders[0].Name);
        Assert.NotEqual(headerValue, collection.DefaultHeaders[0].Value);
        Assert.NotEqual(variableName, collection.Variables[0].Name);
        Assert.NotEqual(variableValue, collection.Variables[0].Value);
    }
}
