using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial record HttpResponseModel(string Status, ImmutableArray<HttpHeaderModel> Headers, HttpContentModel Content) {
    public async static Task<HttpResponseModel> Create(HttpResponseMessage response) {
        // Read content before headers since reading content can add headers like Content-Length
        var content = await GetContent(response.Content);

        return new(
            GetStatus(response.StatusCode),
            [.. response.Headers.Select(HttpHeaderModel.Create), .. response.Content.Headers.Select(HttpHeaderModel.Create)],
            content
        );
    }

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

    // TODO move to HttpContentModel.Create
    private async static Task<HttpContentModel> GetContent(HttpContent content) {
        if (content == null) {
            return new(HttpContentType.None, "", []);
        }

        if (IsText(content.Headers.ContentType)) {
            return new(HttpContentType.Text, await content.ReadAsStringAsync(), []);
        }

        return new(HttpContentType.Unknown, "", [.. await content.ReadAsByteArrayAsync()]);

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
}
