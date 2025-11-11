using System;
using System.Collections.Generic;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public string? DefaultUrl { get; set; }
    public List<RequestTemplate> Requests { get; set; } = [];
    public Dictionary<string, string> Variables { get; set; } = [];
    public List<HeaderTemplate> DefaultHeaders { get; set; } = [];

    public string ApplyVariables(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return value;
        }

        foreach (var variable in Variables) {
            value = value.Replace($"{{{{{variable.Key}}}}}", variable.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        return value;
    }
}
