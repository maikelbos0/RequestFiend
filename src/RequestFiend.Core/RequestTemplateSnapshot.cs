using System;
using System.Collections.Immutable;
using System.Net.Http;

namespace RequestFiend.Core;

public record RequestTemplateSnapshot(
    VariableSnapshot Variables,
    string Name,
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
    public HttpRequestMessage CreateMessage() {
        var message = new HttpRequestMessage(HttpMethod.Parse(Method), new Uri(Variables.Apply(Url)));

        foreach (var header in Headers) {
            message.Headers.Add(Variables.Apply(header.Name), Variables.Apply(header.Value));
        }

        message.Content = ContentManagerProvider.Provide(ContentType).GetContent(this);

        return message;
    }
}
