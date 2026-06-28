using System.Collections.Immutable;

namespace RequestFiend.Core;

public record RequestTemplateSnapshot(
    VariableSnapshot VariableSnapshot,
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
) { }
