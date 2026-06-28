using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class XmlContentManagerTests {
    [Theory]
    [InlineData(false, XmlContentManager.DefaultMediaType)]
    [InlineData(true, null)]
    public async Task GetContent(bool hasManualContentTypeHeader, string? expectedMediaType) {
        var subject = new XmlContentManager();
        var request = new RequestTemplateSnapshot(
            new([
                new("{{TagName}}", "Data"),
                new("{{Value}}", "42")
            ]),
            "Request",
            "POST",
            "https://localhost/",
            [],
            ContentType.Xml,
            hasManualContentTypeHeader,
            "<{{TagName}}>{{Value}}</{{TagName}}>",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var result = Assert.IsType<StringContent>(subject.GetContent(request));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("<Data>42</Data>", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
