using System.Linq;
using System.Net.Http;

namespace RequestFiend.Models;

public record HttpRequestModel(string Method, string? Url, HttpHeaderModel[] Headers) {
    public static HttpRequestModel Create(HttpRequestMessage request) {
        if (request.Content == null) {
            return new(
                request.Method.ToString(),
                request.RequestUri?.ToString(),
                [.. request.Headers.Select(HttpHeaderModel.Create)]
            );
        }

        return new(
            request.Method.ToString(),
            request.RequestUri?.ToString(),
            [.. request.Headers.Select(HttpHeaderModel.Create), .. request.Content.Headers.Select(HttpHeaderModel.Create)]
        );
    }
}
