using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public required string Name { get; set; }
    public List<RequestTemplate> Requests { get; set; } = [];
    public Dictionary<string, string> Variables { get; set; } = [];
    public List<HeaderTemplate> DefaultHeaders { get; set; } = [];

    public bool TryCreateMessage(RequestTemplate request, [NotNullWhen(true)] out HttpRequestMessage? message) {
        if (!Requests.Contains(request)) {
            throw new ArgumentException("This request is not part of the collection.", nameof(request));
        }

        if (!Uri.TryCreate(ApplyVariables(request.Url), UriKind.Absolute, out var uri)) {
            message = null;
            return false;
        }

        message = new(request.Method, uri);
        foreach (var header in request.Headers) {
            message.Headers.Add(ApplyVariables(header.Name), ApplyVariables(header.Value));
        }
        foreach (var header in DefaultHeaders) {
            message.Headers.Add(ApplyVariables(header.Name), ApplyVariables(header.Value));
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
