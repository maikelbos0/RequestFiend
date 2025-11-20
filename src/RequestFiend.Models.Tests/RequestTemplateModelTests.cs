using Microsoft.Maui.Platform;
using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateModelTests {
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

        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };
        var subject = new RequestTemplateModel(request); 

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.Headers[0].Value.Value = headerValue;

        var result = subject.TryUpdateRequestTemplate(request);

        Assert.True(result);
        Assert.Equal(name, request.Name);
        Assert.Equal(method, request.Method);
        Assert.Equal(url, request.Url);
        Assert.Equal(headerName, request.Headers[0].Name);
        Assert.Equal(headerValue, request.Headers[0].Value);
    }

    [Theory]
    [InlineData(null, null, null, null, null)]
    [InlineData(null, "GET", "https://url", "Name", "Value")]
    [InlineData("Name", null, "https://url", "Name", "Value")]
    [InlineData("Name", "GET", null, "Name", "Value")]
    [InlineData("Name", "GET", "https://url", null, "Value")]
    [InlineData("Name", "GET", "https://url", "Name", null)]
    public void TryUpdateRequestTemplate_Fails_When_Invalid(string? name, string? method, string? url, string? headerName, string? headerValue) {
        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };
        var subject = new RequestTemplateModel(request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.Headers[0].Value.Value = headerValue;

        var result = subject.TryUpdateRequestTemplate(request);

        Assert.False(result);
        Assert.NotEqual(name, request.Name);
        Assert.NotEqual(method, request.Method);
        Assert.NotEqual(url, request.Url);
        Assert.NotEqual(headerName, request.Headers[0].Name);
        Assert.NotEqual(headerValue, request.Headers[0].Value);
    }
}
