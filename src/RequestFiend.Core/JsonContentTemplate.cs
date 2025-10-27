using System.Text;
using System.Text.Json;

namespace RequestFiend.Core;

public class JsonContentTemplate : IContentTemplate {
    public const string DefaultMediaType = "application/json";

    public string MediaType { get; set; } = DefaultMediaType;
    public string? CharSet { get; } = Encoding.UTF8.WebName;
    public required string Content { get; set; }

    public bool Validate(RequestTemplateCollection collection) {
        try {
            JsonDocument.Parse(collection.ApplyVariables(Content));
            return true;
        }
        catch {
            return false;
        }
    }

    public bool Format(RequestTemplateCollection collection)
        => throw new NotImplementedException();

    public byte[] GetContent(RequestTemplateCollection collection)
        => Encoding.UTF8.GetBytes(collection.ApplyVariables(Content));
}