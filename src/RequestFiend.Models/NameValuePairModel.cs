using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;

namespace RequestFiend.Models;

public partial class NameValuePairModel : BoundModelBase, IValidatable {
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Value { get; }

    public NameValuePairModel(Func<string, bool> nameValidator) : this("", "", nameValidator) { }

    public NameValuePairModel(string name, string value, Func<string, bool> nameValidator) : this(() => name, () => value, nameValidator) { }

    public NameValuePairModel(NameValuePair pair, Func<string, bool> nameValidator) : this(() => pair.Name, () => pair.Value, nameValidator) { }

    private NameValuePairModel(Func<string> nameProvider, Func<string> valueProvider, Func<string, bool> nameValidator) {
        Name = new(nameProvider, nameValidator);
        Value = new(valueProvider);

        ConfigureState([Name, Value]);
    }

    public void Reset(NameValuePair pair) {
        Name.Reset(() => pair.Name);
        Value.Reset(() => pair.Value);
    }
}
