using System;
using System.Collections.Generic;
using System.Linq;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public static bool IsValidVariableCharacter(char c) => char.IsLetterOrDigit(c) || c == '_';
    public static bool IsValidVariableName(string name) => !string.IsNullOrEmpty(name) && name.All(IsValidVariableCharacter);

    private readonly Dictionary<string, object> sessionData = [];

    public string DefaultUrl { get; set; } = "";
    public List<RequestTemplate> Requests { get; set; } = [];
    public List<NameValuePair> Variables { get; set; } = [];
    public List<NameValuePair> DefaultHeaders { get; set; } = [];

    public Dictionary<string, object> GetSessionData() => sessionData;
    
    public Dictionary<string, string> GetVariables()
        => Variables
            .Where(variable => IsValidVariableName(variable.Name))
            .DistinctBy(variable => variable.Name)
            .ToDictionary(variable => variable.Name, variable => variable.Value);

    public string ApplyVariables(string value) {
        foreach (var (variableName, variableValue) in GetVariables()) {
            value = value.Replace($"{{{{{variableName}}}}}", variableValue, StringComparison.InvariantCultureIgnoreCase);
        }

        return value;
    }
}
