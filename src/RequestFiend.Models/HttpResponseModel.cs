using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace RequestFiend.Models;

public partial record HttpResponseModel(string Status, HttpContentModel Content, ImmutableArray<HttpHeaderModel> Headers) {
    [GeneratedRegex(@"(?<!^)[A-Z][a-z]", RegexOptions.Compiled)]
    private static partial Regex GetWordFinder();

    [GeneratedRegex(@"^application(\/.*\+)?(json|xml)$", RegexOptions.Compiled)]
    private static partial Regex GetApplicationTextMediaTypeFinder();

    private static string GetStatus(HttpStatusCode statusCode) {
        var code = statusCode.ToString("D");
        var name = statusCode.ToString("G");

        if (code == name) {
            return code;
        }

        name = GetWordFinder().Replace(name, m => $" {m.Value}");

        return $"{code} {name}";
    }

    private static HttpContentModel GetContent(HttpContent content) {
        if (content == null) {
            return new(HttpContentType.None, "", []);
        }

        // TODO make it async
        if (IsText(content.Headers.ContentType)) {
            return new(HttpContentType.Text, content.ReadAsStringAsync().GetAwaiter().GetResult(), []);
        }

        return new(HttpContentType.Unknown, "", [.. content.ReadAsByteArrayAsync().GetAwaiter().GetResult()]);

        static bool IsText(MediaTypeHeaderValue? contentType) {
            if (contentType == null) {
                return false;
            }

            if (contentType.MediaType != null && (contentType.MediaType.StartsWith("text/") || GetApplicationTextMediaTypeFinder().IsMatch(contentType.MediaType))) {
                return true;
            }

            return contentType.CharSet != null;
        }
    }

    public HttpResponseModel(HttpResponseMessage response) : this(
        GetStatus(response.StatusCode),
        GetContent(response.Content),
        [.. response.Headers.Select(header => new HttpHeaderModel(header)), .. response.Content.Headers.Select(header => new HttpHeaderModel(header))]
    ) { }
}
