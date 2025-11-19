using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class VariableModel {
    public RequiredString Name { get; set; }
    public RequiredString Value { get; set; }

    public VariableModel() {
        Name = new();
        Value = new();
    }

    public VariableModel(Variable variable) {
        Name = new(() => variable.Name);
        Value = new(() => variable.Value);
    }
}
