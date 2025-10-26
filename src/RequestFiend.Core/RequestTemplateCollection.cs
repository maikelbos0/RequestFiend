using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public required string Name { get; set; }
    public List<RequestTemplate> RequestTemplates { get; set; } = [];
    public Dictionary<string, string> Variables { get; set; } = [];
    public List<HeaderTemplate> DefaultHeaderTemplates { get; set; } = [];

    public bool TryCreateMessage(RequestTemplate requestTemplate, [NotNullWhen(true)] out HttpRequestMessage? message) {
        if (!RequestTemplates.Contains(requestTemplate)) {
            throw new ArgumentException("This template is not part of the collection.", nameof(requestTemplate));
        }

        if (!Uri.TryCreate(ApplyVariables(requestTemplate.Url), UriKind.Absolute, out var uri)) {
            message = null;
            return false;
        }

        message = new(requestTemplate.Method, uri);
        foreach (var headerTemplate in requestTemplate.HeaderTemplates) {
            message.Headers.Add(ApplyVariables(headerTemplate.Name), ApplyVariables(headerTemplate.Value));
        }
        foreach (var headerTemplate in DefaultHeaderTemplates) {
            message.Headers.Add(ApplyVariables(headerTemplate.Name), ApplyVariables(headerTemplate.Value));
        }

        return true;
    }

    public string ApplyVariables(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return value;
        }

        foreach (var variable in Variables) {
            value = value.Replace($"{{{{{variable.Key}}}}}", variable.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        return value;
    }
}
