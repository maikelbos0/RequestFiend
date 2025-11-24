using System;
using System.Net.Http;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateTests {
    [Theory]
    [InlineData(ContentType.None, typeof(NoneContentManager))]
    [InlineData(ContentType.Text, typeof(TextContentManager))]
    [InlineData(ContentType.Json, typeof(JsonContentManager))]
    public void GetContentManager(ContentType type, Type expectedManagerType) {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost",
            ContentType = type
        };

        Assert.Equal(expectedManagerType, subject.GetContentManager().GetType());
    }

    [Theory]
    [InlineData("")]
    [InlineData("localhost")]
    [InlineData("https://")]
    [InlineData("https:localhost")]
    [InlineData("https//localhost")]
    public void TryCreateMessage_Does_Not_Create_Message_When_Url_Is_Invalid(string url) {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = url
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject]
        };

        Assert.False(subject.TryCreateMessage(collection, out var message));
        Assert.Null(message);
    }

    [Fact]
    public void TryCreateMessage_Creates_Message_If_Possible() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject]
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        Assert.NotNull(message);
        Assert.Equal(subject.Method, message.Method.Method);
        Assert.NotNull(message.RequestUri);
        Assert.Equal(subject.Url, message.RequestUri.ToString());
    }

    [Fact]
    public void TryCreateMessage_Applies_Variables_To_Url() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "{{baseurl}}values"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject],
            Variables = { new() { Name = "BaseUrl", Value = "https://localhost:7001/" } }
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        Assert.NotNull(message.RequestUri);
        Assert.Equal("https://localhost:7001/values", message.RequestUri.ToString());
    }

    [Fact]
    public void TryCreateMessage_Adds_Headers() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/",
            Headers = [
                new() { Name = "Accept", Value = "application/json" }
            ]
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject]
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        var header = Assert.Single(message.Headers);
        Assert.Equal("Accept", header.Key);
        Assert.Equal("application/json", Assert.Single(header.Value));
    }

    [Fact]
    public void TryCreateMessage_Applies_Variables_To_Header_Names() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/",
            Headers = [
                new() { Name = "{{header}}", Value = "application/json" }
            ]
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject],
            Variables = { new() { Name = "Header", Value = "Accept" } }
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        var header = Assert.Single(message.Headers);
        Assert.Equal("Accept", header.Key);
    }

    [Fact]
    public void TryCreateMessage_Applies_Variables_To_Header_Values() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/",
            Headers = [
                new() { Name = "Accept", Value = "{{header}}" }
            ]
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject],
            Variables = { new() { Name = "Header", Value = "application/json" } }
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        var header = Assert.Single(message.Headers);
        Assert.Equal("application/json", Assert.Single(header.Value));
    }

    [Fact]
    public void TryCreateMessage_Adds_DefaultHeaders() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject],
            DefaultHeaders = [
                new() { Name = "Accept", Value = "application/json" }
            ]
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        var defaultHeader = Assert.Single(message.Headers);
        Assert.Equal("Accept", defaultHeader.Key);
        Assert.Equal("application/json", Assert.Single(defaultHeader.Value));
    }

    [Fact]
    public void TryCreateMessage_Applies_Variables_To_DefaultHeader_Names() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject],
            Variables = { new() { Name = "DefaultHeader", Value = "Accept" } },
            DefaultHeaders = [
                new() { Name = "{{DefaultHeader}}", Value = "application/json" }
            ]
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        var defaultHeader = Assert.Single(message.Headers);
        Assert.Equal("Accept", defaultHeader.Key);
    }

    [Fact]
    public void TryCreateMessage_Applies_Variables_To_DefaultHeader_Values() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject],
            Variables = { new() { Name = "DefaultHeader", Value = "application/json" } },
            DefaultHeaders = [
                new() { Name = "Accept", Value = "{{DefaultHeader}}" }
            ]
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        var defaultHeader = Assert.Single(message.Headers);
        Assert.Equal("application/json", Assert.Single(defaultHeader.Value));
    }

    [Fact]
    public void TryCreateMessage_Adds_Content_If_Available() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/",
            ContentType = ContentType.Text,
            StringContent = "Just a piece of text"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject]
        };

        Assert.True(subject.TryCreateMessage(collection, out var message));
        Assert.IsType<StringContent>(message.Content);
    }
}
