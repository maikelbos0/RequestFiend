using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RequestFiend.Core;

public class VariableSnapshot {
    public static bool IsValidVariableCharacter(char c) => char.IsLetterOrDigit(c) || c == '_';
    public static bool IsValidVariableName(string name) => !string.IsNullOrEmpty(name) && name.All(IsValidVariableCharacter);

    public ImmutableArray<(string Name, string Value)> Variables { get; }

    public VariableSnapshot(params IEnumerable<NameValuePair>[] variableLists) {
        Variables = [.. variableLists
            .SelectMany(variableList => variableList)
            .Where(variable => IsValidVariableName(variable.Name))
            .DistinctBy(variable => variable.Name)
            .Select(variable => ($"{{{{{variable.Name}}}}}", variable.Value))];
    }

    public string Apply(string value) {
        foreach (var (variableName, variableValue) in Variables) {
            value = value.Replace(variableName, variableValue, StringComparison.InvariantCultureIgnoreCase);
        }

        return value;
    }
}
