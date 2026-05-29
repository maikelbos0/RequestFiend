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
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "First", Value = "Replacement" },
                new() { Name =  "Second", Value = "Another" }
            }
        };

        var result = Assert.IsType<StringContent>(subject.GetContent(request, collection));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("The Replacement and Another get replaced", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
