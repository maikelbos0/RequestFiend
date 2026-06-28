using System;
using System.Net.Http;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateSnapshotTests {
    [Theory]
    [InlineData(ContentType.None, typeof(NoneContentManager))]
    [InlineData(ContentType.Text, typeof(TextContentManager))]
    [InlineData(ContentType.Json, typeof(JsonContentManager))]
    [InlineData(ContentType.Xml, typeof(XmlContentManager))]
    [InlineData(ContentType.File, typeof(FileContentManager))]
    [InlineData(ContentType.FormData, typeof(FormDataContentManager))]
    public void GetContentManager(ContentType contentType, Type expectedManagerType) {
        var subject = new RequestTemplateSnapshot(
            new([]),
            "Request",
            "GET",
            "https://localhost/",
            [],
            contentType,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        Assert.Equal(expectedManagerType, subject.GetContentManager().GetType());
    }

    [Fact]
    public void CreateMessage_Creates_Message() {
        var subject = new RequestTemplateSnapshot(
            new([]),
            "Request",
            "GET",
            "https://localhost/",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var message = subject.CreateMessage();

        Assert.Equal(subject.Method, message.Method.Method);
        Assert.NotNull(message.RequestUri);
        Assert.Equal(subject.Url, message.RequestUri.ToString());
    }

    [Fact]
    public void CreateMessage_Applies_Variables() {
        var subject = new RequestTemplateSnapshot(
            new([
                new("{{BaseUrl}}", "https://localhost:7001/")
            ]),
            "Request",
            "GET",
            "{{baseurl}}values",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var message = subject.CreateMessage();

        Assert.NotNull(message.RequestUri);
        Assert.Equal("https://localhost:7001/values", message.RequestUri.ToString());
    }

    [Fact]
    public void CreateMessage_Adds_Headers_Using_Variables() {
        var subject = new RequestTemplateSnapshot(
            new([
                new("{{HeaderName}}", "Accept"),
                new("{{HeaderValue}}", "application/json")
            ]),
            "Request",
            "GET",
            "https://localhost:7001/",
            [
                new("{{HeaderName}}", "{{HeaderValue}}")
            ],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var message = subject.CreateMessage();

        var header = Assert.Single(message.Headers);
        Assert.Equal("Accept", header.Key);
        Assert.Equal("application/json", Assert.Single(header.Value));
    }

    [Fact]
    public void CreateMessage_Adds_Content_If_Available() {
        var subject = new RequestTemplateSnapshot(
            new([
                new("{{BaseUrl}}", "https://localhost:7001/")
            ]),
            "Request",
            "GET",
            "https://localhost:7001/",
            [],
            ContentType.Text,
            false,
            "Just a piece of text",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var message = subject.CreateMessage();

        Assert.IsType<StringContent>(message.Content);
    }
}
