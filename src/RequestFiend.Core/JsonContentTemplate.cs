using System.Text;
using System.Text.Json;

namespace RequestFiend.Core;

public class JsonContentTemplate : IContentTemplate {
    public const string DefaultMediaType = "application/json";
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    public string MediaType { get; set; } = DefaultMediaType;
    public string? CharSet { get; } = Encoding.UTF8.WebName;
    public required string Content { get; set; }

    public bool Validate(RequestTemplateCollection collection) {
        try {
            _ = JsonDocument.Parse(Content);
            return true;
        }
        catch {
            return false;
        }
    }

    public bool Format(RequestTemplateCollection collection) {
        try {
            var document = JsonDocument.Parse(Content);
            Content = JsonSerializer.Serialize(document, jsonSerializerOptions);

            return true;
        }
        catch {
            return false;
        }
    }

    public byte[] GetContent(RequestTemplateCollection collection)
        => Encoding.UTF8.GetBytes(collection.ApplyVariables(Content));
}