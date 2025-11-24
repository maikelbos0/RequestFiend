using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class TextContentManagerTests {
    [Fact]
    public async Task GetContent() {
        var subject = new TextContentManager();
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost",
            StringContent = "The {{first}} and {{second}} get replaced"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "First", Value = "Replacement" },
                new() { Name =  "Second", Value = "Another" }
            }
        };

        var result = Assert.IsType<StringContent>(subject.GetContent(request, collection));

        Assert.Equal(JsonContentManager.DefaultMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("The Replacement and Another get replaced", await result.ReadAsStringAsync());
    }
}
