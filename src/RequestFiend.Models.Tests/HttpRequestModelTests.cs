using System.Linq;
using System.Net.Http;
using Xunit;

namespace RequestFiend.Models.Tests;

public class HttpRequestModelTests {
    [Fact]
    public void Create() {
        var request = new HttpRequestMessage() {
            Method = HttpMethod.Post,
            RequestUri = new("https://localhost/"),
            Headers = {
                { "Accept", "application/json" }
            }
        };

        var subject = HttpRequestModel.Create(request);

        Assert.Equal(request.Method.ToString(), subject.Method);
        Assert.Equal(request.RequestUri.ToString(), subject.Url);
        Assert.Equal(request.Headers.Count(), subject.Headers.Length);
    }
}
