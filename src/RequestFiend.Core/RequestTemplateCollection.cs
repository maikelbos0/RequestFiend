using System;
using System.Collections.Generic;
using System.Linq;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public static bool IsValidVariableName(string name) => !string.IsNullOrEmpty(name) && !name.Any(c => !char.IsLetterOrDigit(c) && c != '_');

    public string DefaultUrl { get; set; } = "";
    public List<RequestTemplate> Requests { get; set; } = [];
    public List<NameValuePair> DefaultHeaders { get; set; } = [];
    public List<NameValuePair> Variables { get; set; } = [];

    public string ApplyVariables(string value) {
        foreach (var variable in Variables.Where(variable => IsValidVariableName(variable.Name))) {
            value = value.Replace($"{{{{{variable.Name}}}}}", variable.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        return value;
    }
}
