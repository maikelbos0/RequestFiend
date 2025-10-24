using System.Net.Http;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests {
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("localhost")]
    [InlineData("https://")]
    [InlineData("https:localhost")]
    [InlineData("https//localhost")]
    public void TryCreateMessage_Does_Not_Create_Message_When_Url_Is_Invalid(string? url) {
        var requestTemplate = new RequestTemplate() {
            Method = HttpMethod.Get,
            Url = url
        };
        var subject = new RequestTemplateCollection();

        Assert.False(subject.TryCreateMessage(requestTemplate, out var message));
        Assert.Null(message);
    }

    [Fact]
    public void TryCreateMessage_Does_Not_Create_Message_When_Method_Is_Missing() {
        var requestTemplate = new RequestTemplate() {
            Url = "https://localhost:7001"
        };
        var subject = new RequestTemplateCollection();

        Assert.False(subject.TryCreateMessage(requestTemplate, out var message));
        Assert.Null(message);
    }

    [Fact]
    public void TryCreateMessage_Creates_Message_If_Possible() {
        var requestTemplate = new RequestTemplate() {
            Method = HttpMethod.Get,
            Url = "https://localhost:7001/"
        };
        var subject = new RequestTemplateCollection();

        Assert.True(subject.TryCreateMessage(requestTemplate, out var message));

        Assert.NotNull(message);
        Assert.Equal(requestTemplate.Method, message.Method);
        Assert.NotNull(message.RequestUri);
        Assert.Equal(requestTemplate.Url, message.RequestUri.ToString());
    }
}