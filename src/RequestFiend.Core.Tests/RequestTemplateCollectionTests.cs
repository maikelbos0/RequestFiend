using System;
using System.Net.Http;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests {
    [Theory]
    [InlineData("")]
    [InlineData("localhost")]
    [InlineData("https://")]
    [InlineData("https:localhost")]
    [InlineData("https//localhost")]
    public void TryCreateMessage_Does_Not_Create_Message_When_Url_Is_Invalid(string url) {
        var requestTemplate = new RequestTemplate() {
            Name = "Template",
            Method = HttpMethod.Get,
            Url = url
        };
        var subject = new RequestTemplateCollection() {
            Name = "Collection",
            RequestTemplates = [requestTemplate]
        };

        Assert.False(subject.TryCreateMessage(requestTemplate, out var message));
        Assert.Null(message);
    }

    [Fact]
    public void TryCreateMessage_Throws_ArgumentException_If_Template_Not_In_Collection() {
        var requestTemplate = new RequestTemplate() {
            Name = "Template",
            Method = HttpMethod.Get,
            Url = "https://localhost:7001/"
        };
        var subject = new RequestTemplateCollection() {
            Name = "Collection"
        };

        Assert.Equal("requestTemplate", Assert.Throws<ArgumentException>(() => subject.TryCreateMessage(requestTemplate, out var message)).ParamName);
    }

    [Fact]
    public void TryCreateMessage_Creates_Message_If_Possible() {
        var requestTemplate = new RequestTemplate() {
            Name = "Template",
            Method = HttpMethod.Get,
            Url = "https://localhost:7001/"
        };
        var subject = new RequestTemplateCollection() {
            Name = "Collection",
            RequestTemplates = [requestTemplate]
        };

        Assert.True(subject.TryCreateMessage(requestTemplate, out var message));
        Assert.NotNull(message);
        Assert.Equal(requestTemplate.Method, message.Method);
        Assert.NotNull(message.RequestUri);
        Assert.Equal(requestTemplate.Url, message.RequestUri.ToString());
    }

    [Fact]
    public void TryCreateMessage_Applies_Variables_To_Url() {
        var requestTemplate = new RequestTemplate() {
            Name = "Template",
            Method = HttpMethod.Get,
            Url = "{{baseurl}}values"
        };
        var subject = new RequestTemplateCollection() {
            Name = "Collection",
            RequestTemplates = [requestTemplate],
            Variables = { { "BaseUrl", "https://localhost:7001/" } }
        };

        Assert.True(subject.TryCreateMessage(requestTemplate, out var message));
        Assert.Equal(requestTemplate.Method, message.Method);
        Assert.NotNull(message.RequestUri);
        Assert.Equal("https://localhost:7001/values", message.RequestUri.ToString());
    }

    [Fact]
    public void TryCreateMessage_Adds_Headers() {
        var requestTemplate = new RequestTemplate() {
            Name = "Template",
            Method = HttpMethod.Get,
            Url = "https://localhost:7001/",
            HeaderTemplates = [
                new() { Name = "Accept", Value = "application/json" }
            ]
        };
        var subject = new RequestTemplateCollection() {
            Name = "Collection",
            RequestTemplates = [requestTemplate]
        };

        Assert.True(subject.TryCreateMessage(requestTemplate, out var message));
        var header = Assert.Single(message.Headers);
        Assert.Equal("Accept", header.Key);
        Assert.Equal("application/json", Assert.Single(header.Value));
    }

    [Fact]
    public void TryCreateMessage_Applies_Variables_To_Header_Names() {
        var requestTemplate = new RequestTemplate() {
            Name = "Template",
            Method = HttpMethod.Get,
            Url = "https://localhost:7001/",
            HeaderTemplates = [
                new() { Name = "{{header}}", Value = "application/json" }
            ]
        };
        var subject = new RequestTemplateCollection() {
            Name = "Collection",
            RequestTemplates = [requestTemplate],
            Variables = { { "Header", "Accept" } }
        };

        Assert.True(subject.TryCreateMessage(requestTemplate, out var message));
        var header = Assert.Single(message.Headers);
        Assert.Equal("Accept", header.Key);
    }

    [Fact]
    public void TryCreateMessage_Applies_Variables_To_Header_Values() {

        var requestTemplate = new RequestTemplate() {
            Name = "Template",
            Method = HttpMethod.Get,
            Url = "https://localhost:7001/",
            HeaderTemplates = [
                new() { Name = "Accept", Value = "{{header}}" }
            ]
        };
        var subject = new RequestTemplateCollection() {
            Name = "Collection",
            RequestTemplates = [requestTemplate],
            Variables = { { "Header", "application/json" } }
        };

        Assert.True(subject.TryCreateMessage(requestTemplate, out var message));
        var header = Assert.Single(message.Headers);
        Assert.Equal("application/json", Assert.Single(header.Value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void ApplyVariables_Returns_Value_If_Whitespace(string value) {
        var subject = new RequestTemplateCollection() {
            Name = "Collection",
            Variables = {
                { "First", "Replacement" },
                { "Second", "Another" }
            }
        };

        var result = subject.ApplyVariables(value);

        Assert.Equal(value, result);
    }

    [Fact]
    public void ApplyVariables_Replaces_Variables_With_Values() {
        var subject = new RequestTemplateCollection() {
            Name = "Collection",
            Variables = {
                { "First", "Replacement" },
                { "Second", "Another" }
            }
        };

        var result = subject.ApplyVariables("{{First}} first and {{second}}");

        Assert.Equal("Replacement first and Another", result);
    }
}
