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
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "POST",
            Url = "https://localhost",
            HasManualContentTypeHeader = hasManualContentTypeHeader,
            StringContent = "The {{first}} and {{second}} get replaced"
        };
        var variableSnapshot = new VariableSnapshot([
            new("{{First}}", "Replacement"),
            new("{{Second}}", "Another")
        ]);

        var result = Assert.IsType<StringContent>(subject.GetContent(request, variableSnapshot));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("The Replacement and Another get replaced", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
