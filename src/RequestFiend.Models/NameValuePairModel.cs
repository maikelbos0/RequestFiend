using CommunityToolkit.Mvvm.ComponentModel;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class NameValuePairModel : ObservableObject {
    public ValidatableString Name { get; set; }
    public ValidatableString Value { get; set; }

    public NameValuePairModel() : this(new(true), new(true)) { }

    public NameValuePairModel(NameValuePair pair) : this(new(true, () => pair.Name), new(true, () => pair.Value)) { }

    private NameValuePairModel(ValidatableString name, ValidatableString value) {
        Name = name;
        Value = value;
    }

    public void Reinitialize(NameValuePair pair) {
        Name.Reinitialize(() => pair.Name);
        Value.Reinitialize(() => pair.Value);
    }
}
