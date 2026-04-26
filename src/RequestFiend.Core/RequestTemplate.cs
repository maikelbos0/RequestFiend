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
    public string StringContent { get; set; } = "";
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
        _ => throw new NotImplementedException($"Received unknown content type '{ContentType}'.")
    };

    public HttpRequestMessage CreateMessage(RequestTemplateCollection collection) {
        var message = new HttpRequestMessage(HttpMethod.Parse(Method), new Uri(collection.ApplyVariables(Url)));

        foreach (var header in Headers) {
            message.Headers.Add(collection.ApplyVariables(header.Name), collection.ApplyVariables(header.Value));
        }
        foreach (var header in collection.DefaultHeaders) {
            message.Headers.Add(collection.ApplyVariables(header.Name), collection.ApplyVariables(header.Value));
        }

        message.Content = GetContentManager().GetContent(this, collection);

        return message;
    }
}
