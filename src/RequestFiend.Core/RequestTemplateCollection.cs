using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public required string Name { get; set; }
    public List<RequestTemplate> Templates { get; set; } = [];
    public Dictionary<string, string> Variables { get; set; } = [];

    public bool TryCreateMessage(RequestTemplate template, [NotNullWhen(true)] out HttpRequestMessage? message) {
        if (!Templates.Contains(template)) {
            throw new ArgumentException("This template is not part of the collection.", nameof(template));
        }

        if (!Uri.TryCreate(ApplyVariables(template.Url), UriKind.Absolute, out var uri)) {
            message = null;
            return false;
        }

        message = new(template.Method, uri);

        return true;
    }

    public string? ApplyVariables(string? value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return value;
        }

        foreach (var variable in Variables) {
            value = value.Replace($"{{{{{variable.Key}}}}}", variable.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        return value;
    }
}
