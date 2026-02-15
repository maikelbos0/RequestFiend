using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class JsonContentManagerTests {
    [Fact]
    public async Task GetContent() {
        var subject = new JsonContentManager();
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost",
            StringContent = "[{{Node}}, {{Node}}]"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Node", Value = "{\"Meaning\": 42}" }
            }
        };

        var result = Assert.IsType<StringContent>(subject.GetContent(request, collection));

        Assert.Equal(JsonContentManager.DefaultMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("[{\"Meaning\": 42}, {\"Meaning\": 42}]", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetContent_Without_StringContent(string? stringContent) {
        var subject = new JsonContentManager();
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost",
            StringContent = stringContent
        };
        var collection = new RequestTemplateCollection();

        Assert.Null(subject.GetContent(request, collection));
    }
}
