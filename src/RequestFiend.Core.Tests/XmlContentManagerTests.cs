using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class XmlContentManagerTests {
    [Fact]
    public async Task GetContent() {
        var subject = new XmlContentManager();
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "POST",
            Url = "https://localhost",
            StringContent = "<{{TagName}}>{{Value}}</{{TagName}}>"
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "TagName", Value = "Data" },
                new() { Name = "Value", Value = "42" }
            }
        };

        var result = Assert.IsType<StringContent>(subject.GetContent(request, collection));

        Assert.Equal(XmlContentManager.DefaultMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("<Data>42</Data>", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
