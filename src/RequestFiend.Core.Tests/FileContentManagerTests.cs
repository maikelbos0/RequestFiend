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
        File.WriteAllBytes("./Data.json", [0, 1, 2, 3]);
        var subject = new FileContentManager();
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "POST",
            Url = "https://localhost",
            HasManualContentTypeHeader = hasManualContentTypeHeader,
            FileContent = "./{{FileName}}"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "FileName", Value = "Data.json" }
            }
        };

        var result = Assert.IsType<ByteArrayContent>(subject.GetContent(request, collection));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal([0, 1, 2, 3], await result.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }
}
