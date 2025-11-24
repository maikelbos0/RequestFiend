using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateModelTests {
    [Theory]
    [InlineData(ContentType.None, false, false)]
    [InlineData(ContentType.Text, true, false)]
    [InlineData(ContentType.Json, true, true)]
    public void SetContentType(ContentType contentType, bool expectedUsesStringContent, bool expectedUsesJsonContent) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url"
        };
        var subject = new RequestTemplateModel(request);

        subject.ContentType = contentType;

        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesJsonContent, subject.UsesJsonContent);
    }

    [Fact]
    public void Constructor() {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url"
        };
        var subject = new RequestTemplateModel(request);

        Assert.Equal(request.Name, subject.Name.Value);
        Assert.Equal(request.Method, subject.Method.Value);
        Assert.Equal(request.Url, subject.Url.Value);
    }

    [Fact]
    public void TryUpdateRequestTemplate() {
        const string name = "Name";
        const string method = "GET";
        const string url = "https://url";
        const string headerName = "Name";
        const string headerValue = "Value";
        const ContentType contentType = ContentType.Json;
        const string stringContent = "Content";

        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            Content = {
                Type = ContentType.Text,
                StringContent = "PreviousContent"
            }
        };
        var subject = new RequestTemplateModel(request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.Headers[0].Value.Value = headerValue;
        subject.ContentType = contentType;
        subject.StringContent.Value = stringContent;

        var result = subject.TryUpdateRequestTemplate(request);

        Assert.True(result);
        Assert.Equal(name, request.Name);
        Assert.Equal(method, request.Method);
        Assert.Equal(url, request.Url);
        Assert.Equal(headerName, request.Headers[0].Name);
        Assert.Equal(headerValue, request.Headers[0].Value);
        Assert.Equal(contentType, request.Content.Type);
        Assert.Equal(stringContent, request.Content.StringContent);
    }

    [Theory]
    [InlineData(null, null, null, null, null)]
    [InlineData(null, "GET", "https://url", "Name", "Value")]
    [InlineData("Name", null, "https://url", "Name", "Value")]
    [InlineData("Name", "GET", null, "Name", "Value")]
    [InlineData("Name", "GET", "https://url", null, "Value")]
    [InlineData("Name", "GET", "https://url", "Name", null)]
    public void TryUpdateRequestTemplate_Fails_When_Invalid(string? name, string? method, string? url, string? headerName, string? headerValue) {
        const ContentType contentType = ContentType.Json;
        const string stringContent = "Content";

        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            Content = {
                Type = ContentType.Text,
                StringContent = "PreviousContent"
            }
        };
        var subject = new RequestTemplateModel(request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.Headers[0].Value.Value = headerValue;
        subject.ContentType = contentType;
        subject.StringContent.Value = stringContent;

        var result = subject.TryUpdateRequestTemplate(request);

        Assert.False(result);
        Assert.NotEqual(name, request.Name);
        Assert.NotEqual(method, request.Method);
        Assert.NotEqual(url, request.Url);
        Assert.NotEqual(headerName, request.Headers[0].Name);
        Assert.NotEqual(headerValue, request.Headers[0].Value);
        Assert.NotEqual(contentType, request.Content.Type);
        Assert.NotEqual(stringContent, request.Content.StringContent);
    }
}
