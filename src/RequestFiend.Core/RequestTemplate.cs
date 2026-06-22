using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace RequestFiend.Core;

public class RequestTemplate {
    public required string Name { get; set; }
    public required string Method { get; set; }
    public required string Url { get; set; }
    public List<NameValuePair> Headers { get; set; } = [];
    public ContentType ContentType { get; set; } = ContentType.None;
    public bool HasManualContentTypeHeader { get; set; }
    public string StringContent { get; set; } = "";
    public string FileContent { get; set; } = "";
    public List<NameValuePair> FormFieldContent { get; set; } = [];
    public List<NameValuePair> FormFileContent { get; set; } = [];
    public Script PreExchangeScript { get; set; } = new();
    public Script PostExchangeScript { get; set; } = new();
    public Script OnExceptionScript { get; set; } = new();

    public RequestTemplate Clone()
        => new() {
            Name = Name,
            Method = Method,
            Url = Url,
            Headers = [.. Headers.Select(header => header.Clone())],
            ContentType = ContentType,
            StringContent = StringContent,
            PreExchangeScript = PreExchangeScript.Clone(),
            PostExchangeScript = PostExchangeScript.Clone(),
            OnExceptionScript = OnExceptionScript.Clone()
        };

    public IContentManager GetContentManager() => ContentType switch {
        ContentType.None => new NoneContentManager(),
        ContentType.Text => new TextContentManager(),
        ContentType.Json => new JsonContentManager(),
        ContentType.Xml => new XmlContentManager(),
        ContentType.File => new FileContentManager(),
        ContentType.FormData => new FormDataContentManager(),
        _ => throw new NotImplementedException($"Received unknown content type '{ContentType}'.")
    };

    public HttpRequestMessage CreateMessage(RequestTemplateCollection collection, VariableSnapshot variableSnapshot) {
        var message = new HttpRequestMessage(HttpMethod.Parse(Method), new Uri(variableSnapshot.Apply(Url)));

        foreach (var header in Headers) {
            message.Headers.Add(variableSnapshot.Apply(header.Name), variableSnapshot.Apply(header.Value));
        }
        foreach (var header in collection.DefaultHeaders) {
            message.Headers.Add(variableSnapshot.Apply(header.Name), variableSnapshot.Apply(header.Value));
        }

        message.Content = GetContentManager().GetContent(this, variableSnapshot);

        return message;
    }
}
