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
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "POST",
            Url = "https://localhost",
            HasManualContentTypeHeader = hasManualContentTypeHeader,
            StringContent = "[{{Node}}, {{Node}}]"
        };
        var variableSnapshot = new VariableSnapshot([
            new("{{Node}}", "{\"Meaning\": 42}")
        ]);

        var result = Assert.IsType<StringContent>(subject.GetContent(request, variableSnapshot));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("[{\"Meaning\": 42}, {\"Meaning\": 42}]", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
