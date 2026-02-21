using CommunityToolkit.Mvvm.ComponentModel;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;

namespace RequestFiend.Models;

public class NameValuePairModel : ObservableObject {
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Value { get; }

    public NameValuePairModel() : this(() => "", () => "") { }

    public NameValuePairModel(NameValuePair pair) : this(() => pair.Name, () => pair.Value) { }

    private NameValuePairModel(Func<string> nameProvider, Func<string> valueProvider) {
        Name = new(nameProvider, Validator.Required);
        Value = new(valueProvider, Validator.Required);
    }

    public void Reset(NameValuePair pair) {
        Name.Reset(() => pair.Name);
        Value.Reset(() => pair.Value);
    }
}
