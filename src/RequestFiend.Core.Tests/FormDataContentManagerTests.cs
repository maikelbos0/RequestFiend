using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class FormDataContentManagerTests {
    [Theory]
    [InlineData(false, "multipart/form-data")]
    [InlineData(true, null)]
    public async Task GetContent(bool hasManualContentTypeHeader, string? expectedMediaType) {
        var subject = new FormDataContentManager();
        var request = new RequestTemplateSnapshot(
            new([
                new("{{FileName}}", "Data.json"),
                new("{{First}}", "Replacement"),
                new("{{Second}}", "Another")
            ]),
            "Request",
            "POST",
            "https://localhost/",
            [],
            ContentType.FormData,
            hasManualContentTypeHeader,
            "StringContent",
            "FileContent",
            [
                new("Description", "The {{first}} and {{second}} get replaced")
            ],
            [
                new("Data", "./{{FileName}}")
            ],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var result = Assert.IsType<MultipartFormDataContent>(subject.GetContent(request));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("The Replacement and Another get replaced", await Assert.IsType<StringContent>(Assert.Single(result, content => content.Headers.ContentDisposition?.Name == "Description")).ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal(File.ReadAllBytes("./Data.json"), await Assert.IsType<ByteArrayContent>(Assert.Single(result, content => content.Headers.ContentDisposition?.Name == "Data")).ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }
}
