using System.Linq;
using System.Net.Http;
using Xunit;

namespace RequestFiend.Models.Tests;

public class HttpRequestModelTests {
    [Fact]
    public void Create_Without_Content() {
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

    [Fact]
    public void Create_With_Content() {
        var request = new HttpRequestMessage() {
            Method = HttpMethod.Post,
            RequestUri = new("https://localhost/"),
            Headers = {
                { "Accept", "application/json" }
            },
            Content = new StringContent("Content")
        };

        var subject = HttpRequestModel.Create(request);

        Assert.Equal(request.Method.ToString(), subject.Method);
        Assert.Equal(request.RequestUri.ToString(), subject.Url);
        Assert.Equal(request.Headers.Count() + request.Content.Headers.Count(), subject.Headers.Length);
    }
}
