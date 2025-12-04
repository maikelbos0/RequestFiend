using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Services;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NewRequestTemplateModelTests {
    [Fact]
    public void Constructor() {
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://default"
        };
        var subject = new NewRequestTemplateModel(Substitute.For<IFileService>(), @"C:\Documents\External data requests.json", collection);

        Assert.Equal(collection.DefaultUrl, subject.Url.Value);
    }

    [Fact]
    public void TryCreateRequestTemplate() {
        const string name = "Name";
        const string method = "GET";
        const string url = "https://url";

        var subject = new NewRequestTemplateModel(Substitute.For<IFileService>(), @"C:\Documents\External data requests.json", new RequestTemplateCollection());

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;

        var result = subject.TryCreateRequestTemplate(out var request);

        Assert.True(result);
        Assert.NotNull(request);
        Assert.Equal(name, request.Name);
        Assert.Equal(method, request.Method);
        Assert.Equal(url, request.Url);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, "GET", "https://url")]
    [InlineData("Name", null, "https://url")]
    [InlineData("Name", "GET", null)]
    public void TryCreateRequestTemplate_Fails_When_Invalid(string? name, string? method, string? url) {
        var subject = new NewRequestTemplateModel(Substitute.For<IFileService>(), @"C:\Documents\External data requests.json", new RequestTemplateCollection());

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;

        var result = subject.TryCreateRequestTemplate(out var request);

        Assert.False(result);
        Assert.Null(request);
    }
}

