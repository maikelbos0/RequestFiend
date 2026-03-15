using System.Collections.Generic;

namespace RequestFiend.Models;

public record HttpHeaderModel(string Name, string Value) {
    public HttpHeaderModel(KeyValuePair<string, IEnumerable<string>> header) : this(header.Key, string.Join(", ", header.Value)) { }
}