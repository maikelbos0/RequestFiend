using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace RequestFiend.Models.Tests;

public class UrlModelTests {
    [Theory]
    [InlineData("Foo", "Foo")]
    [InlineData("Foo + Bar", "Foo+%2b+Bar")]
    [InlineData("{Bar}", "%7bBar%7d")]
    [InlineData("{Bar}}", "%7bBar%7d%7d")]
    [InlineData("{{Bar}", "%7b%7bBar%7d")]
    [InlineData("{{Bar}}", "{{Bar}}")]
    [InlineData("Foo + {{Bar}} + Baz", "Foo+%2b+{{Bar}}+%2b+Baz")]
    [InlineData("{{{Bar}}}", "%7b{{Bar}}%7d")]
    public void EncodeUrlComponent(string uriComponent, string expectedResult) {
        var result = UrlModel.EncodeUrlComponent(uriComponent);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("Foo", "Foo")]
    [InlineData("Foo+%2b+Bar", "Foo + Bar")]
    [InlineData("%7bBar%7d", "{Bar}")]
    [InlineData("%7bBar%7d%7d", "{Bar}}")]
    [InlineData("%7b%7bBar%7d", "{{Bar}")]
    [InlineData("{{Bar}}", "{{Bar}}")]
    [InlineData("Foo+%2b+{{Bar}}+%2b+Baz", "Foo + {{Bar}} + Baz")]
    [InlineData("%7b{{Bar}}%7d", "{{{Bar}}}")]
    public void DecodeUrlComponent(string uriComponent, string expectedResult) {
        var result = HttpUtility.UrlDecode(uriComponent);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("https://localhost/api")]
    [InlineData("https://localhost/api?")]
    public void Constructor_Without_Parameters(string url) {
        var subject = new UrlModel(Substitute.For<Func<string?, CancellationToken, Task>>(), url);

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Empty(subject.Parameters);
    }

    [Fact]
    public void Constructor_With_Parameters() {
        var subject = new UrlModel(Substitute.For<Func<string?, CancellationToken, Task>>(), "https://localhost/api?Foo&%7bBar%7d=Test+%2b+{{Qux}}&Baz");

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Equal(3, subject.Parameters.Count);
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Foo" && pair.Value.Value == "");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "{Bar}" && pair.Value.Value == "Test + {{Qux}}");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Baz" && pair.Value.Value == "");
    }

    [Fact]
    public void ParseQueryStringFromBaseUrl() {
        var subject = new UrlModel(Substitute.For<Func<string?, CancellationToken, Task>>(), "https://localhost/api?Foo") {
            BaseUrl = {
                Value = "https://localhost/api?%7bBar%7d=Test+%2b+{{Qux}}&Baz"
            }
        };

        subject.ParseQueryStringFromBaseUrl();

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Equal(3, subject.Parameters.Count);
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Foo" && pair.Value.Value == "");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "{Bar}" && pair.Value.Value == "Test + {{Qux}}");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Baz" && pair.Value.Value == "");
    }

    [Theory]
    [InlineData(false, "https://localhost/api?Foo=")]
    [InlineData(true, "https://localhost/api?Foo=&%7bBar%7d=Test+%2b+{{Qux}}&Baz=")]
    public async Task Confirm(bool addParameters, string expectedUrl) {
        var closeMethod = Substitute.For<Func<string?, CancellationToken, Task>>();

        var subject = new UrlModel(closeMethod, "https://localhost/api?Foo");

        if (addParameters) {
            subject.Parameters.Add("{Bar}", "Test + {{Qux}}");
            subject.Parameters.Add("Baz", "");
        }

        await subject.Confirm(CancellationToken.None);

        await closeMethod.Received().Invoke(expectedUrl, CancellationToken.None);
    }

    [Fact]
    public async Task Confirm_Fails_When_Invalid() {
        var closeMethod = Substitute.For<Func<string?, CancellationToken, Task>>();

        var subject = new UrlModel(closeMethod, "https://localhost/api?Foo") {
            HasError = true
        };

        await subject.Confirm(CancellationToken.None);

        await closeMethod.DidNotReceive().Invoke(Arg.Any<string?>(), CancellationToken.None);
    }

    [Fact]
    public async Task Cancel() {
        var closeMethod = Substitute.For<Func<string?, CancellationToken, Task>>();

        var subject = new UrlModel(closeMethod, "https://localhost/api?Foo");

        await subject.Cancel(CancellationToken.None);

        await closeMethod.Received().Invoke(null, CancellationToken.None);
    }
}
