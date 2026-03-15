using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;

namespace RequestFiend.Models;

public record HttpRequestModel(string Name, ImmutableArray<HttpHeaderModel> Headers) {
    public HttpRequestModel(HttpRequestMessage request) : this($"{request.Method} {request.RequestUri}", [.. request.Headers.Select(header => new HttpHeaderModel(header))]) { }
}
