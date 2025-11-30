using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateModelTests {
    [Theory]
    [InlineData(null, false, true)]
    [InlineData("Name", false, false)]
    [InlineData("NewName", true, false)]
    public void Name(string? name, bool expectedIsModified, bool expectedHasError) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url"
        };
        var subject = new RequestTemplateModel(request) {
            Name = { Value = name }
        };

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData(null, false, true)]
    [InlineData("GET", false, false)]
    [InlineData("POST", true, false)]
    public void Method(string? method, bool expectedIsModified, bool expectedHasError) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url"
        };
        var subject = new RequestTemplateModel(request) {
            Method = { Value = method }
        };

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData(null, false, true)]
    [InlineData("https://url", false, false)]
    [InlineData("https://newurl", true, false)]
    public void Url(string? url, bool expectedIsModified, bool expectedHasError) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url"
        };
        var subject = new RequestTemplateModel(request) {
            Url = { Value = url }
        };

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData(null, false, true)]
    [InlineData("Value", false, false)]
    [InlineData("NewValue", true, false)]
    public void Headers(string? value, bool expectedIsModified, bool expectedHasError) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url",
            Headers = {
                new() { Name = "Name", Value = "Value" }
            }
        };
        var subject = new RequestTemplateModel(request);

        subject.Headers[0].Value.Value = value;

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("Content", false)]
    [InlineData("New Content", true)]
    public void StringContent(string? stringContent, bool expectedIsModified) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url",
            StringContent = "Content"
        };
        var subject = new RequestTemplateModel(request) {
            StringContent = { Value = stringContent }
        };

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.False(subject.HasError);
    }

    [Theory]
    [InlineData(Core.ContentType.None, false, false)]
    [InlineData(Core.ContentType.Text, true, false)]
    [InlineData(Core.ContentType.Json, true, true)]
    public void ContentType(ContentType contentType, bool expectedUsesStringContent, bool expectedUsesJsonContent) {
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
        Assert.False(subject.IsModified);
        Assert.False(subject.HasError);
    }

    [Fact]
    public void TryUpdateRequestTemplate() {
        const string name = "Name";
        const string method = "GET";
        const string url = "https://url";
        const string headerName = "Name";
        const string headerValue = "Value";
        const ContentType contentType = Core.ContentType.Json;
        const string stringContent = "Content";

        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            ContentType = Core.ContentType.Text,
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
        Assert.False(subject.Name.IsModified);
        Assert.False(subject.Method.IsModified);
        Assert.False(subject.Url.IsModified);
        Assert.False(subject.Headers[0].Name.IsModified);
        Assert.False(subject.Headers[0].Value.IsModified);
        Assert.False(subject.StringContent.IsModified);
        Assert.False(subject.IsModified);
        Assert.False(subject.HasError);
    }

    [Theory]
    [InlineData(null, null, null, null, null)]
    [InlineData(null, "GET", "https://url", "Name", "Value")]
    [InlineData("Name", null, "https://url", "Name", "Value")]
    [InlineData("Name", "GET", null, "Name", "Value")]
    [InlineData("Name", "GET", "https://url", null, "Value")]
    [InlineData("Name", "GET", "https://url", "Name", null)]
    public void TryUpdateRequestTemplate_Fails_When_Invalid(string? name, string? method, string? url, string? headerName, string? headerValue) {
        const ContentType contentType = Core.ContentType.Json;
        const string stringContent = "Content";

        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            ContentType = Core.ContentType.Text,
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
        Assert.True(subject.IsModified);
        Assert.True(subject.HasError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Text", false)]
    [InlineData("\"Field\":\"Value\"", false)]
    [InlineData("{\"Object\":{\"Field\":\"Value\"}}", true)]
    [InlineData("{\"Array\":[0,1,2,3,4,5]}", true)]
    public void ValidateJson(string? stringContent, bool expectedResult) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url",
            ContentType = Core.ContentType.Json,
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
    [InlineData(null, true, null)]
    [InlineData("", true, "")]
    [InlineData("Text", false, "Text")]
    [InlineData("\"Field\":\"Value\"", false, "\"Field\":\"Value\"")]
    [InlineData("{\"Object\":{\"Field\":\"Value\"}}", true, "{\r\n  \"Object\": {\r\n    \"Field\": \"Value\"\r\n  }\r\n}")]
    [InlineData("{\"Array\":[0,1,2,3,4,5]}", true, "{\r\n  \"Array\": [\r\n    0,\r\n    1,\r\n    2,\r\n    3,\r\n    4,\r\n    5\r\n  ]\r\n}")]
    public void FormatJson(string? stringContent, bool expectedResult, string? expectedStringContent) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url",
            ContentType = Core.ContentType.Json,
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
