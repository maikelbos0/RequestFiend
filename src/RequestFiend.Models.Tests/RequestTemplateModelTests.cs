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
        var subject = new RequestTemplateModel(request) {
            ContentType = contentType
        };

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
            ContentType = ContentType.Text,
            StringContent = "PreviousContent"
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
        Assert.Equal(contentType, request.ContentType);
        Assert.Equal(stringContent, request.StringContent);
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
            ContentType = ContentType.Text,
            StringContent = "PreviousContent"
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
        Assert.NotEqual(contentType, request.ContentType);
        Assert.NotEqual(stringContent, request.StringContent);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("Text", false)]
    [InlineData("\"Field\":\"Value\"", false)]
    [InlineData("{\"Object\":{\"Field\":\"Value\"}}", true)]
    [InlineData("{\"Array\":[0,1,2,3,4,5]}", true)]
    public void ValidateJson(string? stringContent, bool expectedResult) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url",
            ContentType = ContentType.Json,
            StringContent = stringContent
        };
        var subject = new RequestTemplateModel(request);

        Assert.Equal(expectedResult, subject.ValidateJson(out var exception));

        if (expectedResult) {
            Assert.Null(exception);
        }
        else {
            Assert.NotNull(exception);
        }
    }

    [Theory]
    [InlineData(null, false, null)]
    [InlineData("Text", false, "Text")]
    [InlineData("\"Field\":\"Value\"", false, "\"Field\":\"Value\"")]
    [InlineData("{\"Object\":{\"Field\":\"Value\"}}", true, "{\r\n  \"Object\": {\r\n    \"Field\": \"Value\"\r\n  }\r\n}")]
    [InlineData("{\"Array\":[0,1,2,3,4,5]}", true, "{\r\n  \"Array\": [\r\n    0,\r\n    1,\r\n    2,\r\n    3,\r\n    4,\r\n    5\r\n  ]\r\n}")]
    public void FormatJson(string? stringContent, bool expectedResult, string? expectedStringContent) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url",
            ContentType = ContentType.Json,
            StringContent = stringContent
        };
        var subject = new RequestTemplateModel(request);

        Assert.Equal(expectedResult, subject.FormatJson(out var exception));
        Assert.Equal(subject.StringContent.Value, expectedStringContent);

        if (expectedResult) {
            Assert.Null(exception);
        }
        else {
            Assert.NotNull(exception);
        }
    }
}
