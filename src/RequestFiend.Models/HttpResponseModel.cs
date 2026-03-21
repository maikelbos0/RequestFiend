using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace RequestFiend.Models;

public partial record HttpResponseModel(string Status, ImmutableArray<HttpHeaderModel> Headers) {
    [GeneratedRegex(@"(?<!^)[A-Z][a-z]", RegexOptions.Compiled)]
    private static partial Regex GetWordFinder();

    private static string GetStatus(HttpStatusCode statusCode) {
        var code = statusCode.ToString("D");
        var name = statusCode.ToString("G");

        if (code == name) {
            return code;
        }

        name = GetWordFinder().Replace(name, m => $" {m.Value}");

        return $"{code} {name}";
    }

    public HttpResponseModel(HttpResponseMessage response) : this(
        GetStatus(response.StatusCode),
        [.. response.Headers.Select(header => new HttpHeaderModel(header))]
    ) { }
}
