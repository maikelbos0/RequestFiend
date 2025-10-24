using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public string? Name { get; set; }
    public List<RequestTemplate> Templates { get; set; } = [];

    public bool TryCreateMessage(RequestTemplate template, [NotNullWhen(true)] out HttpRequestMessage? message) {
        if (!Uri.TryCreate(template.Url, UriKind.Absolute, out var uri) 
            || template.Method == null) {
            message = null;
            return false;
        }

        message = new(template.Method, uri);

        return true;
    }
}
