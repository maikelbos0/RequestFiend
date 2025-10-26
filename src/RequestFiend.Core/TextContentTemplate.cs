using System.Text;

namespace RequestFiend.Core;

public class TextContentTemplate : IContentTemplate {
    public const string DefaultMediaType = "text/plain";

    public string MediaType { get; set; } = DefaultMediaType;
    public string? CharSet { get; } = Encoding.UTF8.WebName;
    public required string Text { get; set; }

    public bool Validate(RequestTemplateCollection collection)
        => true;

    public void Format(RequestTemplateCollection collection) { }

    public byte[] GetContent(RequestTemplateCollection collection)
        => Encoding.UTF8.GetBytes(collection.ApplyVariables(Text));
}
