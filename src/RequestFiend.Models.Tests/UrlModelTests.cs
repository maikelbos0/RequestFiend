using NSubstitute;
using RequestFiend.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class UrlModelTests {
    [Theory]
    [InlineData("https://localhost/api")]
    [InlineData("https://localhost/api?")]
    public void Constructor_Without_Parameters(string url) {
        var collection = new RequestTemplateCollection();

        var subject = new UrlModel(Substitute.For<Func<string?, CancellationToken, Task>>(), collection, url);

        Assert.Equal(collection, subject.Collection);

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Empty(subject.Parameters);

        Assert.Equal([subject.BaseUrl, subject.Parameters], subject.Validatables);
    }

    [Fact]
    public void Constructor_With_Parameters() {
        var subject = new UrlModel(Substitute.For<Func<string?, CancellationToken, Task>>(), new(), "https://localhost/api?Foo&%7bBar%7d=Test%2b{{Qux}}&Baz");

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Equal(3, subject.Parameters.Count);
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Foo" && pair.Value.Value == "");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "{Bar}" && pair.Value.Value == "Test+{{Qux}}");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Baz" && pair.Value.Value == "");

        Assert.Equal([subject.BaseUrl, subject.Parameters], subject.Validatables);
    }

    [Fact]
    public void ParseQueryStringFromBaseUrl_Without_Parameters() {
        var subject = new UrlModel(Substitute.For<Func<string?, CancellationToken, Task>>(), new(), "https://localhost/api?Foo");

        subject.ParseQueryStringFromBaseUrl();

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Single(subject.Parameters);
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Foo" && pair.Value.Value == "");
        Assert.False(subject.IsModified);
    }

    [Fact]
    public void ParseQueryStringFromBaseUrl_With_Parameters() {
        var subject = new UrlModel(Substitute.For<Func<string?, CancellationToken, Task>>(), new(), "https://localhost/api?Foo") {
            BaseUrl = {
                Value = "https://localhost/api?%7bBar%7d=Test%2b{{Qux}}&Baz"
            }
        };

        subject.ParseQueryStringFromBaseUrl();

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Equal(3, subject.Parameters.Count);
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Foo" && pair.Value.Value == "");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "{Bar}" && pair.Value.Value == "Test+{{Qux}}");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Baz" && pair.Value.Value == "");
        Assert.True(subject.IsModified);
    }

    [Theory]
    [InlineData("https://localhost/api?Foo", "https://localhost/api", false, "https://localhost/api?Foo=")]
    [InlineData("https://localhost/api?Foo", "https://localhost/api", true, "https://localhost/api?Foo=&%7bBar%7d=Test%2b{{Qux}}&Baz=")]
    [InlineData("https://localhost/api?Foo", "https://localhost/api?", false, "https://localhost/api?Foo=")]
    [InlineData("https://localhost/api", "https://localhost/api?", true, "https://localhost/api?%7bBar%7d=Test%2b{{Qux}}&Baz=")]
    [InlineData("https://localhost/api", "https://localhost/api?Bar", false, "https://localhost/api?Bar")]
    [InlineData("https://localhost/api", "https://localhost/api", true, "https://localhost/api?%7bBar%7d=Test%2b{{Qux}}&Baz=")]
    [InlineData("https://localhost/api?Foo", "https://localhost/api?Bar", false, "https://localhost/api?Bar&Foo=")]
    public async Task Confirm(string url, string baseUrl, bool addParameters, string expectedUrl) {
        var closeMethod = Substitute.For<Func<string?, CancellationToken, Task>>();

        var subject = new UrlModel(closeMethod, new(), url) {
            BaseUrl = { Value = baseUrl }
        };

        if (addParameters) {
            subject.Parameters.Add("{Bar}", "Test+{{Qux}}");
            subject.Parameters.Add("Baz", "");
        }

        await subject.Confirm(CancellationToken.None);

        await closeMethod.Received(1).Invoke(expectedUrl, CancellationToken.None);
    }

    [Fact]
    public async Task Confirm_Fails_When_Invalid() {
        var closeMethod = Substitute.For<Func<string?, CancellationToken, Task>>();

        var subject = new UrlModel(closeMethod, new(), "https://localhost/api?Foo") {
            HasError = true
        };

        await subject.Confirm(CancellationToken.None);

        await closeMethod.DidNotReceive().Invoke(Arg.Any<string?>(), CancellationToken.None);
    }

    [Fact]
    public async Task Cancel() {
        var closeMethod = Substitute.For<Func<string?, CancellationToken, Task>>();

        var subject = new UrlModel(closeMethod, new(), "https://localhost/api?Foo");

        await subject.Cancel(CancellationToken.None);

        await closeMethod.Received(1).Invoke(null, CancellationToken.None);
    }
}
