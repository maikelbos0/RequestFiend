using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;

namespace RequestFiend.Models;

public record HttpRequestModel(string Method, string? Url, ImmutableArray<HttpHeaderModel> Headers) {
    public static HttpRequestModel Create(HttpRequestMessage request)
        => new(request.Method.ToString(), request.RequestUri?.ToString(), [.. request.Headers.Select(HttpHeaderModel.Create)]);
}
