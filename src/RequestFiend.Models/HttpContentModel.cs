using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial record HttpContentModel(HttpContentType Type, string? TextContent, byte[]? BinaryContent) {
    private static readonly HashSet<string> imageMediaTypes = ["image/bmp", "image/gif", "image/jpeg", "image/png", "image/svg+xml"];
    private static readonly HashSet<HttpContentType> TextContentTypes = [HttpContentType.Text];
    private static readonly HashSet<HttpContentType> BinaryContentTypes = [HttpContentType.Image, HttpContentType.Unknown];

    public async static Task<HttpContentModel> Create(HttpContent? content) {
        if (content == null) {
            // TODO what other ways are there?
            return new(HttpContentType.None, null, null);
        }

        var type = GetType(content.Headers.ContentType);
        var textContent = TextContentTypes.Contains(type) ? await content.ReadAsStringAsync() : null;
        var binaryContent = BinaryContentTypes.Contains(type) ? await content.ReadAsByteArrayAsync() : null;

        return new(type, textContent, binaryContent);
    }

    private static HttpContentType GetType(MediaTypeHeaderValue? contentType) {
        if (contentType == null) {
            return HttpContentType.Unknown;
        }

        if (contentType.MediaType != null && (contentType.MediaType.StartsWith("text/") || GetApplicationTextMediaTypeFinder().IsMatch(contentType.MediaType))) {
            return HttpContentType.Text;
        }

        if (contentType.CharSet != null) {
            return HttpContentType.Text;
        }

        if (contentType.MediaType != null && imageMediaTypes.Contains(contentType.MediaType)) {
            return HttpContentType.Image;
        }

        return HttpContentType.Unknown;
    }

    [GeneratedRegex(@"^application\/(.*\+)?(json|xml)$", RegexOptions.Compiled)]
    private static partial Regex GetApplicationTextMediaTypeFinder();
};
