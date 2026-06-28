using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class TextContentManagerTests {
    [Theory]
    [InlineData(false, "text/plain")]
    [InlineData(true, null)]
    public async Task GetContent(bool hasManualContentTypeHeader, string? expectedMediaType) {
        var subject = new TextContentManager();
        var request = new RequestTemplateSnapshot(
            new([
                new("{{First}}", "Replacement"),
                new("{{Second}}", "Another")
            ]),
            "Request",
            "POST",
            "https://localhost/",
            [],
            ContentType.Text,
            hasManualContentTypeHeader,
            "The {{first}} and {{second}} get replaced",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var result = Assert.IsType<StringContent>(subject.GetContent(request));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("The Replacement and Another get replaced", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
