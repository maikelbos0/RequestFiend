using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class NameValuePairModel {
    public RequiredString Name { get; set; }
    public RequiredString Value { get; set; }
    public bool IsValid => Name.IsValid && Value.IsValid;

    public NameValuePairModel() {
        Name = new();
        Value = new();
    }

    public NameValuePairModel(NameValuePair pair) {
        Name = new(() => pair.Name);
        Value = new(() => pair.Value);
    }
}
