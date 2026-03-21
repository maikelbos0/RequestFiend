using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace RequestFiend.Models.Tests;

public class HttpResponseModelTests {
    [Theory]
    [InlineData(HttpStatusCode.OK, "200 OK")]
    [InlineData(HttpStatusCode.IMUsed, "226 IM Used")]
    [InlineData(HttpStatusCode.BadRequest, "400 Bad Request")]
    public void Constructor(HttpStatusCode statusCode, string expectedStatus) {
        var response = new HttpResponseMessage() {
            StatusCode = statusCode,
            Headers = {
                { "Accept", "application/json" }
            }
        };

        var subject = new HttpResponseModel(response);

        Assert.Equal(expectedStatus, subject.Status);
        Assert.Equal(response.Headers.Count(), subject.Headers.Length);
    }
}
