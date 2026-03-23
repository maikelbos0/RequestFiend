using RequestFiend.Core;
using System.Collections.Generic;
using System.Linq;

namespace RequestFiend.Models;

public static class Options {
    public static List<string> Methods { get; } = ["GET", "PUT", "POST", "DELETE", "HEAD", "OPTIONS", "TRACE", "PATCH"];
    public static Dictionary<ContentType, string> ContentTypeMap { get; } = new() {
        [ContentType.None] = "None",
        [ContentType.Text] = "Text",
        [ContentType.Json] = "JSON"
    };
    public static Dictionary<string, ContentType> ReverseContentTypeMap { get; } = ContentTypeMap.ToDictionary(pair => pair.Value, pair => pair.Key);
    public static List<string> ContentTypes { get; } = [.. ContentTypeMap.Values];
}
