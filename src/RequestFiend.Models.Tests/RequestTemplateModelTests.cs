using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateModelTests {
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
            ContentType = { Value = Options.ContentTypeMap[contentType] }
        };

        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesJsonContent, subject.UsesJsonContent);
    }

    [Theory]
    [InlineData(Core.ContentType.None, false, false)]
    [InlineData(Core.ContentType.Text, true, false)]
    [InlineData(Core.ContentType.Json, true, true)]
    public void Constructor(ContentType contentType, bool expectedUsesStringContent, bool expectedUsesJsonContent) {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url",
            Headers = {
                new() { Name = "Name", Value = "Value" }
            },
            ContentType = contentType,
            StringContent = "Content"
        };
        var subject = new RequestTemplateModel(request);

        Assert.Equal(request.Name, subject.Name.Value);
        Assert.Equal(request.Method, subject.Method.Value);
        Assert.Equal(request.Url, subject.Url.Value);
        Assert.Equal(request.Headers[0].Name, subject.Headers[0].Name.Value);
        Assert.Equal(request.Headers[0].Value, subject.Headers[0].Value.Value);
        Assert.Equal(Options.ContentTypeMap[request.ContentType], subject.ContentType.Value);
        Assert.Equal(request.StringContent, subject.StringContent.Value);
        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesJsonContent, subject.UsesJsonContent);
    }

    [Fact]
    public void TryUpdateRequestTemplate() {
        const string name = "Name";
        const string method = "GET";
        const string url = "https://url";
        const string headerName = "Name";
        const string headerValue = "Value";
        const string contentType = "JSON";
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
        subject.ContentType.Value = contentType;
        subject.StringContent.Value = stringContent;

        var result = subject.TryUpdateRequestTemplate(request);

        Assert.True(result);
        Assert.Equal(name, request.Name);
        Assert.Equal(method, request.Method);
        Assert.Equal(url, request.Url);
        Assert.Equal(headerName, request.Headers[0].Name);
        Assert.Equal(headerValue, request.Headers[0].Value);
        Assert.Equal(Options.ReverseContentTypeMap[contentType], request.ContentType);
        Assert.Equal(stringContent, request.StringContent);
        Assert.False(subject.Name.IsModified);
        Assert.False(subject.Method.IsModified);
        Assert.False(subject.Url.IsModified);
        Assert.False(subject.Headers[0].Name.IsModified);
        Assert.False(subject.Headers[0].Value.IsModified);
        Assert.False(subject.StringContent.IsModified);
    }

    [Theory]
    [InlineData(null, null, null, null, null, null)]
    [InlineData(null, "GET", "https://url", "Name", "Value", "JSON")]
    [InlineData("Name", null, "https://url", "Name", "Value", "JSON")]
    [InlineData("Name", "GET", null, "Name", "Value", "JSON")]
    [InlineData("Name", "GET", "https://url", null, "Value", "JSON")]
    [InlineData("Name", "GET", "https://url", "Name", null, "JSON")]
    [InlineData("Name", "GET", "https://url", "Name", "Value", null)]
    public void TryUpdateRequestTemplate_Fails_When_Invalid(string? name, string? method, string? url, string? headerName, string? headerValue, string? contentType) {
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
        subject.ContentType.Value = contentType;
        subject.StringContent.Value = stringContent;

        var result = subject.TryUpdateRequestTemplate(request);

        Assert.False(result);
        Assert.Equal("Old", request.Name);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://previous", request.Url);
        Assert.Equal("PreviousName", request.Headers[0].Name);
        Assert.Equal("PreviousValue", request.Headers[0].Value);
        Assert.Equal(Core.ContentType.Text, request.ContentType);
        Assert.Equal("PreviousContent", request.StringContent);
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
