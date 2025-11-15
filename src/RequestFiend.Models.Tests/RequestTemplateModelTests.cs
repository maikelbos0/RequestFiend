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

        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous"
        };
        var subject = new RequestTemplateModel(request); 

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;

        var result = subject.TryUpdateRequestTemplate(request);

        Assert.True(result);
        Assert.Equal(name, request.Name);
        Assert.Equal(method, request.Method);
        Assert.Equal(url, request.Url);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, "GET", "https://url")]
    [InlineData("Name", null, "https://url")]
    [InlineData("Name", "GET", null)]
    public void TryUpdateRequestTemplate_Fails_When_Invalid(string? name, string? method, string? url) {
        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous"
        };
        var subject = new RequestTemplateModel(request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;

        var result = subject.TryUpdateRequestTemplate(request);

        Assert.False(result);
        Assert.NotEqual(name, request.Name);
        Assert.NotEqual(method, request.Method);
        Assert.NotEqual(url, request.Url);
    }
}
