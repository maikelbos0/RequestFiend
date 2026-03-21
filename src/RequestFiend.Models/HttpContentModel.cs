using System.Collections.Immutable;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial record HttpContentModel(HttpContentType Type, string? TextContent = null, ImmutableArray<byte>? BinaryContent = null) {
    public async static Task<HttpContentModel> Create(HttpContent? content) {
        if (content == null) {
            // TODO what other ways are there?
            return new(HttpContentType.None, null, null);
        }

        if (IsText(content.Headers.ContentType)) {
            return new(HttpContentType.Text, TextContent: await content.ReadAsStringAsync());
        }

        return new(HttpContentType.Unknown, BinaryContent: [.. await content.ReadAsByteArrayAsync()]);
    }

    private static bool IsText(MediaTypeHeaderValue? contentType) {
        if (contentType == null) {
            return false;
        }

        if (contentType.MediaType != null && (contentType.MediaType.StartsWith("text/") || GetApplicationTextMediaTypeFinder().IsMatch(contentType.MediaType))) {
            return true;
        }

        return contentType.CharSet != null;
    }

    [GeneratedRegex(@"^application\/(.*\+)?(json|xml)$", RegexOptions.Compiled)]
    private static partial Regex GetApplicationTextMediaTypeFinder();

    public bool HasContent => Type != HttpContentType.None;
};
