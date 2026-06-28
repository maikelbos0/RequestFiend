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
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "POST",
            Url = "https://localhost",
            HasManualContentTypeHeader = hasManualContentTypeHeader,
            FileContent = "./{{FileName}}"
        };
        var variableSnapshot = new VariableSnapshot([
            new("{{FileName}}", "Data.json")
        ]);

        var result = Assert.IsType<ByteArrayContent>(subject.GetContent(request, variableSnapshot));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal(File.ReadAllBytes("./Data.json"), await result.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }
}
