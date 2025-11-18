using System;
using System.Collections.Generic;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public string? DefaultUrl { get; set; }
    public List<RequestTemplate> Requests { get; set; } = [];
    public List<Variable> Variables { get; set; } = [];
    public List<HeaderTemplate> DefaultHeaders { get; set; } = [];

    public string ApplyVariables(string value) {
        foreach (var variable in Variables) {
            value = value.Replace($"{{{{{variable.Name}}}}}", variable.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        return value;
    }
}
