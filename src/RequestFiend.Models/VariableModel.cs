using System.Collections.Immutable;
using System.Linq;

namespace RequestFiend.Models;

public record VariableModel(string Key, string Value) {
    public static VariableModel[] CreateRange(ImmutableDictionary<string, string> variables)
        => [.. variables.Select(variable => new VariableModel(variable.Key, variable.Value))];
}
