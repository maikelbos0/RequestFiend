using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class XmlContentManagerTests {
    [Theory]
    [InlineData(false, XmlContentManager.DefaultMediaType)]
    [InlineData(true, null)]
    public async Task GetContent(bool hasManualContentTypeHeader, string? expectedMediaType) {
        var subject = new XmlContentManager();
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "POST",
            Url = "https://localhost",
            HasManualContentTypeHeader = hasManualContentTypeHeader,
            StringContent = "<{{TagName}}>{{Value}}</{{TagName}}>"
        };
        var variableSnapshot = new VariableSnapshot([
            new NameValuePair() { Name = "TagName", Value = "Data" },
            new NameValuePair() { Name = "Value", Value = "42" }
        ]);

        var result = Assert.IsType<StringContent>(subject.GetContent(request, variableSnapshot));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("<Data>42</Data>", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
