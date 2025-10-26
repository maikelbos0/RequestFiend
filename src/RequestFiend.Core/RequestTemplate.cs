using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Core;

public class RequestTemplate
{
    public required string Name { get; set; }
    public required HttpMethod Method { get; set; }
    public required string Url { get; set; }
    public List<HeaderTemplate> Headers { get; set; } = [];
    public IContentTemplate? Content { get; set; }

    public bool TryCreateMessage(RequestTemplateCollection collection, [NotNullWhen(true)] out HttpRequestMessage? message) {
        if (!Uri.TryCreate(collection.ApplyVariables(Url), UriKind.Absolute, out var uri)) {
            message = null;
            return false;
        }

        message = new(Method, uri);
        foreach (var header in Headers) {
            message.Headers.Add(collection.ApplyVariables(header.Name), collection.ApplyVariables(header.Value));
        }
        foreach (var header in collection.DefaultHeaders) {
            message.Headers.Add(collection.ApplyVariables(header.Name), collection.ApplyVariables(header.Value));
        }

        if (Content != null) {
            message.Content = new ByteArrayContent(Content.GetContent(collection)) {
                Headers = {
                      ContentType = new(Content.MediaType, Content.CharSet)
                }
            };
        }

        return true;
    }
}
