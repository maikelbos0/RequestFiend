using System;
using System.Collections.Immutable;
using System.Net.Http;

namespace RequestFiend.Core;

public record RequestTemplateSnapshot(
    VariableSnapshot Variables,
    string Method,
    string Url,
    ImmutableArray<NameValuePairSnapshot> Headers,
    ContentType ContentType,
    bool HasManualContentTypeHeader,
    string StringContent,
    string FileContent,
    ImmutableArray<NameValuePairSnapshot> FormFieldContent,
    ImmutableArray<NameValuePairSnapshot> FormFileContent,
    ScriptSnapshot PreExchangeScript,
    ScriptSnapshot PostExchangeScript,
    ScriptSnapshot OnExceptionScript
) {
    public IContentManager GetContentManager() => ContentType switch {
        ContentType.None => new NoneContentManager(),
        ContentType.Text => new TextContentManager(),
        ContentType.Json => new JsonContentManager(),
        ContentType.Xml => new XmlContentManager(),
        ContentType.File => new FileContentManager(),
        ContentType.FormData => new FormDataContentManager(),
        _ => throw new NotImplementedException($"Received unknown content type '{ContentType}'.")
    };

    public HttpRequestMessage CreateMessage() {
        var message = new HttpRequestMessage(HttpMethod.Parse(Method), new Uri(Variables.Apply(Url)));

        foreach (var header in Headers) {
            message.Headers.Add(Variables.Apply(header.Name), Variables.Apply(header.Value));
        }

        message.Content = GetContentManager().GetContent(this);

        return message;
    }
}
