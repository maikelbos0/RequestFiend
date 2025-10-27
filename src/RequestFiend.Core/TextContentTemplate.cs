using System.Text;

namespace RequestFiend.Core;

public class TextContentTemplate : IContentTemplate {
    public const string DefaultMediaType = "text/plain";

    public string MediaType { get; set; } = DefaultMediaType;
    public string? CharSet { get; } = Encoding.UTF8.WebName;
    public required string Content { get; set; }

    public bool Validate(RequestTemplateCollection collection)
        => true;

    public bool Format(RequestTemplateCollection collection)
        => true;

    public byte[] GetContent(RequestTemplateCollection collection)
        => Encoding.UTF8.GetBytes(collection.ApplyVariables(Content));
}
