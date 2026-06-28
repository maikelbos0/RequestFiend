using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class FileContentManagerTests {
    [Theory]
    [InlineData(false, "application/json")]
    [InlineData(true, null)]
    public async Task GetContent(bool hasManualContentTypeHeader, string? expectedMediaType) {
        var subject = new FileContentManager();
        var request = new RequestTemplateSnapshot(
            new([
                new("{{FileName}}", "Data.json")
            ]),
            "Request",
            "POST",
            "https://localhost/",
            [],
            ContentType.File,
            hasManualContentTypeHeader,
            "StringContent",
            "./{{FileName}}",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var result = Assert.IsType<ByteArrayContent>(subject.GetContent(request));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal(File.ReadAllBytes("./Data.json"), await result.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }
}
