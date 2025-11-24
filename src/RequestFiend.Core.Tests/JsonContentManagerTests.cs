using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class JsonContentManagerTests {
    [Fact]
    public async Task GetContent() {
        var subject = new JsonContentManager();
        var content = new ContentTemplate() {
            StringContent = "[{{Node}}, {{Node}}]"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Node", Value = "{\"Meaning\": 42}" }
            }
        };

        var result = Assert.IsType<StringContent>(subject.GetContent(content, collection));

        Assert.Equal(JsonContentManager.DefaultMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("[{\"Meaning\": 42}, {\"Meaning\": 42}]", await result.ReadAsStringAsync());
    }
}
