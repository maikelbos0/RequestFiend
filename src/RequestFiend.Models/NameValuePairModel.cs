using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;

namespace RequestFiend.Models;

public partial class NameValuePairModel : BoundModelBase, IValidatable {
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Value { get; }

    public NameValuePairModel(Func<string> nameProvider, Func<string> valueProvider, Func<string, bool> nameValidator, params IValidatable[] dependencies)
        : this(nameProvider, valueProvider, nameValidator, _ => true, dependencies) { }

    public NameValuePairModel(Func<string> nameProvider, Func<string> valueProvider, Func<string, bool> nameValidator, Func<string, bool> valueValidator, params IValidatable[] dependencies) {
        Name = new(nameProvider, nameValidator, dependencies);
        Value = new(valueProvider, valueValidator, dependencies);

        ConfigureState([Name, Value]);
    }

    public void Reset(NameValuePair pair) {
        Name.Reset(() => pair.Name);
        Value.Reset(() => pair.Value);
    }

    public NameValuePair GetNameValuePair()
        => new() { Name = Name.Value, Value = Value.Value };
}
