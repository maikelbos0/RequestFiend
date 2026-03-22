using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial record HttpContentModel(HttpContentType Type, string? TextContent, byte[]? BinaryContent, string? HexContent) {
    private static readonly HashSet<string> imageMediaTypes = ["image/bmp", "image/gif", "image/jpeg", "image/png", "image/svg+xml"];
    private static readonly HashSet<HttpContentType> textContentTypes = [HttpContentType.Text];
    private static readonly HashSet<HttpContentType> binaryContentTypes = [HttpContentType.Image, HttpContentType.Unknown];
    private static readonly string hexStringLookup = Convert.ToHexString([.. Enumerable.Range(byte.MinValue, byte.MaxValue - byte.MinValue + 1).Select(i => (byte)i)]);

    public async static Task<HttpContentModel> Create(HttpContent? content) {
        if (content == null) {
            // TODO what other ways are there?
            return new(HttpContentType.None, null, null, null);
        }

        var type = GetType(content.Headers.ContentType);
        var textContent = textContentTypes.Contains(type) ? await content.ReadAsStringAsync() : null;
        var binaryContent = binaryContentTypes.Contains(type) ? await content.ReadAsByteArrayAsync() : null;

        return new(type, textContent, binaryContent, TranslateToHexContent(binaryContent));
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

    public static string? TranslateToHexContent(byte[]? binaryContent) {
        if (binaryContent == null) {
            return null;
        }

        const int spacePeriod = 4;
        var characters = new char[binaryContent.Length * 2 + (binaryContent.Length - 1) / 4];
        var spaceCount = 0;

        for (int i = 0; i < binaryContent.Length; i++) {
            if (i > 0 && i % spacePeriod == 0) {
                characters[i * 2 + spaceCount++] = ' ';
            }

            characters[i * 2 + spaceCount] = hexStringLookup[binaryContent[i] * 2];
            characters[i * 2 + spaceCount + 1] = hexStringLookup[binaryContent[i] * 2 + 1];
        }

        return new string(characters);
    }

    [GeneratedRegex(@"^application\/(.*\+)?(json|xml)$", RegexOptions.Compiled)]
    private static partial Regex GetApplicationTextMediaTypeFinder();
};
