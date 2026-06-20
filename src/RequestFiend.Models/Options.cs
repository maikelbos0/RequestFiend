using RequestFiend.Core;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RequestFiend.Models;

public static class Options {
    public static ImmutableDictionary<ScriptEvaluationMode, string> ScriptEvaluationModeMap { get; } = ImmutableDictionary.CreateRange([
        KeyValuePair.Create(ScriptEvaluationMode.Disabled, "Disabled"),
        KeyValuePair.Create(ScriptEvaluationMode.Enabled, "Enabled"),
        KeyValuePair.Create(ScriptEvaluationMode.CollectionScoped, "Scoped per collection")
    ]);
    public static ImmutableDictionary<string, ScriptEvaluationMode> ReverseScriptEvaluationModeMap { get; } = ScriptEvaluationModeMap.ToImmutableDictionary(pair => pair.Value, pair => pair.Key);
    public static ImmutableArray<string> ScriptEvaluationModes { get; } = [.. ScriptEvaluationModeMap.Values];
    public static ImmutableArray<string> Methods { get; } = ["GET", "PUT", "POST", "DELETE", "HEAD", "OPTIONS", "TRACE", "PATCH"];
    public static ImmutableDictionary<ContentType, string> ContentTypeMap { get; } = ImmutableDictionary.CreateRange([
        KeyValuePair.Create(ContentType.None, "None"),
        KeyValuePair.Create(ContentType.Text, "Text"),
        KeyValuePair.Create(ContentType.Json, "JSON"),
        KeyValuePair.Create(ContentType.Xml, "XML"),
        KeyValuePair.Create(ContentType.File, "File"),
        KeyValuePair.Create(ContentType.FormData, "Multipart form data")
    ]);
    public static ImmutableDictionary<string, ContentType> ReverseContentTypeMap { get; } = ContentTypeMap.ToImmutableDictionary(pair => pair.Value, pair => pair.Key);
    public static ImmutableArray<string> ContentTypes { get; } = [.. ContentTypeMap.Values];
}
