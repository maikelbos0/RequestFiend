using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial record HttpResponseModel(string Status, ImmutableArray<HttpHeaderModel> Headers, HttpContentModel Content) {
    public async static Task<HttpResponseModel> Create(HttpResponseMessage response) {
        // Read content before headers since reading content can add content headers like Content-Length
        var content = await HttpContentModel.Create(response.Content);

        return new(
            GetStatus(response.StatusCode),
            [.. response.Headers.Select(HttpHeaderModel.Create), .. response.Content.Headers.Select(HttpHeaderModel.Create)],
            content
        );
    }

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
}
