using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;

namespace RequestFiend.Models;

public record HttpRequestModel(string Method, string? Url, ImmutableArray<HttpHeaderModel> Headers) {
    public HttpRequestModel(HttpRequestMessage request) : this(
        request.Method.ToString(), 
        request.RequestUri?.ToString(),
        [.. request.Headers.Select(header => new HttpHeaderModel(header))]
    ) { }
}
