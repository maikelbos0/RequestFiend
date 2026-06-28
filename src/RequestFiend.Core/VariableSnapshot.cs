using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RequestFiend.Core;

public record VariableSnapshot(ImmutableDictionary<string, string> Variables) {
    public static bool IsValidVariableCharacter(char c) => char.IsLetterOrDigit(c) || c == '_';
    public static bool IsValidVariableName(string name) => !string.IsNullOrEmpty(name) && name.All(IsValidVariableCharacter);

    public static VariableSnapshot Create(params IEnumerable<NameValuePair>[] variableLists)
        => new(variableLists
            .SelectMany(variableList => variableList)
            .Where(variable => IsValidVariableName(variable.Name))
            .DistinctBy(variable => variable.Name)
            .ToImmutableDictionary(variable => $"{{{{{variable.Name}}}}}", variable => variable.Value));

    public string Apply(string value) {
        foreach (var (variableName, variableValue) in Variables) {
            value = value.Replace(variableName, variableValue, StringComparison.InvariantCultureIgnoreCase);
        }

        return value;
    }
}
