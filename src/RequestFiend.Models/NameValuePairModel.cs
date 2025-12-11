using CommunityToolkit.Mvvm.ComponentModel;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class NameValuePairModel : ObservableObject {
    public ValidatableString Name { get; set; }
    public ValidatableString Value { get; set; }

    public NameValuePairModel() : this(new(ValidationMode.Required), new(ValidationMode.Required)) { }

    public NameValuePairModel(NameValuePair pair) : this(new(ValidationMode.Required, () => pair.Name), new(ValidationMode.Required, () => pair.Value)) { }

    private NameValuePairModel(ValidatableString name, ValidatableString value) {
        Name = name;
        Value = value;
    }

    public void Reinitialize(NameValuePair pair) {
        Name.Reinitialize(() => pair.Name);
        Value.Reinitialize(() => pair.Value);
    }
}
