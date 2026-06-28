using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class JsonContentManagerTests {
    [Theory]
    [InlineData(false, JsonContentManager.DefaultMediaType)]
    [InlineData(true, null)]
    public async Task GetContent(bool hasManualContentTypeHeader, string? expectedMediaType) {
        var subject = new JsonContentManager();
        var request = new RequestTemplateSnapshot(
            new([
                new("{{Node}}", "{\"Meaning\": 42}")
            ]),
            "Request",
            "POST",
            "https://localhost/",
            [],
            ContentType.Json,
            hasManualContentTypeHeader,
            "[{{Node}}, {{Node}}]",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var result = Assert.IsType<StringContent>(subject.GetContent(request));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("[{\"Meaning\": 42}, {\"Meaning\": 42}]", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
