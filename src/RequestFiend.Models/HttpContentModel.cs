using System.Collections.Immutable;

namespace RequestFiend.Models;

public record HttpContentModel(HttpContentType Type, string TextContent, ImmutableArray<byte> BinaryContent) {
    public bool HasContent => Type != HttpContentType.None;
};
