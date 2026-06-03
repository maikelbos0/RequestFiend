using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;

namespace RequestFiend.Models;

public partial class NameValuePairModel : BoundModelBase, IValidatable {
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Value { get; }

    public NameValuePairModel(Func<string> nameProvider, Func<string> valueProvider, Func<string, bool> nameValidator)
        : this(nameProvider, valueProvider, nameValidator, _ => true) { }

    public NameValuePairModel(Func<string> nameProvider, Func<string> valueProvider, Func<string, bool> nameValidator, Func<string, bool> valueValidator) {
        Name = new(nameProvider, nameValidator);
        Value = new(valueProvider, valueValidator);

        ConfigureState([Name, Value]);
    }

    public void Reset(NameValuePair pair) {
        Name.Reset(() => pair.Name);
        Value.Reset(() => pair.Value);
    }

    public NameValuePair GetNameValuePair()
        => new() { Name = Name.Value, Value = Value.Value };
}
