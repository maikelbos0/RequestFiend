using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class HttpResponseModelTests {
    [Theory]
    [InlineData(HttpStatusCode.OK, "200 OK")]
    [InlineData(HttpStatusCode.IMUsed, "226 IM Used")]
    [InlineData(HttpStatusCode.BadRequest, "400 Bad Request")]
    public async Task Create(HttpStatusCode statusCode, string expectedStatus) {
        var response = new HttpResponseMessage() {
            StatusCode = statusCode,
            Headers = {
                { "Cache-control", "max-age=0, private" }
            },
            Content = new StringContent("String content")
        };

        var subject = await HttpResponseModel.Create(response);

        Assert.Equal(expectedStatus, subject.Status);
        Assert.Equal(response.Headers.Count() + response.Content.Headers.Count(), subject.Headers.Length);
    }
}
