using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace RequestFiend.Core;

public class RequestTemplate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Method { get; set; }
    public required string Url { get; set; }
    public List<NameValuePair> Headers { get; set; } = [];
    public ContentTemplate Content { get; set; } = new();

    public bool TryCreateMessage(RequestTemplateCollection collection, [NotNullWhen(true)] out HttpRequestMessage? message) {
        if (!Uri.TryCreate(collection.ApplyVariables(Url), UriKind.Absolute, out var uri)) {
            message = null;
            return false;
        }

        message = new(HttpMethod.Parse(Method), uri);
        foreach (var header in Headers) {
            message.Headers.Add(collection.ApplyVariables(header.Name), collection.ApplyVariables(header.Value));
        }
        foreach (var header in collection.DefaultHeaders) {
            message.Headers.Add(collection.ApplyVariables(header.Name), collection.ApplyVariables(header.Value));
        }

        if (Content != null) {
            message.Content = Content.GetContent(collection);
        }

        return true;
    }
}
