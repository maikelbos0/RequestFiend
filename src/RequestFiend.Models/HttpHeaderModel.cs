using System.Collections.Generic;

namespace RequestFiend.Models;

public record HttpHeaderModel(string Name, string Value) {
    public static HttpHeaderModel Create(KeyValuePair<string, IEnumerable<string>> header)
        => new(header.Key, string.Join(", ", header.Value));
}