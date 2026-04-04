using System;
using System.Net.Http;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateTests {
    [Fact]
    public void Clone() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/",
            Headers = [
                new() { Name = "Accept", Value = "application/json" }
            ],
            ContentType = ContentType.Text,
            StringContent = "Just a piece of text",
            PreExchangeScript = "PreExchangeScript",
            PostExchangeScript = "PostExchangeScript",
            OnExceptionScript = "OnExceptionScript"
        };

        var result = subject.Clone();

        Assert.Equal(subject.Name, result.Name);
        Assert.Equal(subject.Method, result.Method);
        Assert.Equal(subject.Url, result.Url);
        Assert.NotSame(subject.Headers, result.Headers);
        Assert.Equal(subject.Headers.Count, result.Headers.Count);
        Assert.Equal(subject.ContentType, result.ContentType);
        Assert.Equal(subject.StringContent, result.StringContent);
        Assert.Equal(subject.PreExchangeScript, result.PreExchangeScript);
        Assert.Equal(subject.PostExchangeScript, result.PostExchangeScript);
        Assert.Equal(subject.OnExceptionScript, result.OnExceptionScript);
    }

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

    [Fact]
    public void CreateMessage_Creates_Message() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject]
        };

        var message = subject.CreateMessage(collection);

        Assert.Equal(subject.Method, message.Method.Method);
        Assert.NotNull(message.RequestUri);
        Assert.Equal(subject.Url, message.RequestUri.ToString());
    }

    [Fact]
    public void CreateMessage_Applies_Variables_To_Url() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "{{baseurl}}values"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [subject],
            Variables = { new() { Name = "BaseUrl", Value = "https://localhost:7001/" } }
        };

        var message = subject.CreateMessage(collection);

        Assert.NotNull(message.RequestUri);
        Assert.Equal("https://localhost:7001/values", message.RequestUri.ToString());
    }

    [Fact]
    public void CreateMessage_Adds_Headers() {
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

        var message = subject.CreateMessage(collection);

        var header = Assert.Single(message.Headers);
        Assert.Equal("Accept", header.Key);
        Assert.Equal("application/json", Assert.Single(header.Value));
    }

    [Fact]
    public void CreateMessage_Applies_Variables_To_Header_Names() {
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

        var message = subject.CreateMessage(collection);

        var header = Assert.Single(message.Headers);
        Assert.Equal("Accept", header.Key);
    }

    [Fact]
    public void CreateMessage_Applies_Variables_To_Header_Values() {
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

        var message = subject.CreateMessage(collection);

        var header = Assert.Single(message.Headers);
        Assert.Equal("application/json", Assert.Single(header.Value));
    }

    [Fact]
    public void CreateMessage_Adds_DefaultHeaders() {
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

        var message = subject.CreateMessage(collection);

        var defaultHeader = Assert.Single(message.Headers);
        Assert.Equal("Accept", defaultHeader.Key);
        Assert.Equal("application/json", Assert.Single(defaultHeader.Value));
    }

    [Fact]
    public void CreateMessage_Applies_Variables_To_DefaultHeader_Names() {
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

        var message = subject.CreateMessage(collection);

        var defaultHeader = Assert.Single(message.Headers);
        Assert.Equal("Accept", defaultHeader.Key);
    }

    [Fact]
    public void CreateMessage_Applies_Variables_To_DefaultHeader_Values() {
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

        var message = subject.CreateMessage(collection);

        var defaultHeader = Assert.Single(message.Headers);
        Assert.Equal("application/json", Assert.Single(defaultHeader.Value));
    }

    [Fact]
    public void CreateMessage_Adds_Content_If_Available() {
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

        var message = subject.CreateMessage(collection);

        Assert.IsType<StringContent>(message.Content);
    }
}
