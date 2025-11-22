using System.Net.Http;
using System.Text.Json;

namespace RequestFiend.Core;

public class JsonContentManager : IContentManager {
    public const string DefaultMediaType = "text/plain";

    public static JsonSerializerOptions JsonSerializerOptions { get; } = new() { WriteIndented = true };

    public bool Validate(ContentTemplate content, RequestTemplateCollection collection) {
        try {
            _ = JsonDocument.Parse(content.StringContent);
            return true;
        }
        catch {
            return false;
        }
    }

    public bool Format(ContentTemplate content, RequestTemplateCollection collection) {
        try {
            var document = JsonDocument.Parse(content.StringContent);
            content.StringContent = JsonSerializer.Serialize(document, JsonSerializerOptions);

            return true;
        }
        catch {
            return false;
        }
    }

    public HttpContent? GetContent(ContentTemplate content, RequestTemplateCollection collection)
        => new StringContent(collection.ApplyVariables(content.StringContent), null, DefaultMediaType);
}
