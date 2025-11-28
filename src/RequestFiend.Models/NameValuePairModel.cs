using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class NameValuePairModel {
    public ValidatableString Name { get; set; }
    public ValidatableString Value { get; set; }
    public bool HasError => Name.HasError || Value.HasError;

    public NameValuePairModel() {
        Name = new(true);
        Value = new(true);
    }

    public NameValuePairModel(NameValuePair pair) {
        Name = new(true, () => pair.Name);
        Value = new(true, () => pair.Value);
    }

    public void Reinitialize(NameValuePair pair) {
        Name.Reinitialize(() => pair.Name);
        Value.Reinitialize(() => pair.Value);
    }
}
