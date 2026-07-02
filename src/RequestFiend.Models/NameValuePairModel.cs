using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;

namespace RequestFiend.Models;

public partial class NameValuePairModel : BoundModelBase, IValidatable {
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Value { get; }

    public NameValuePairModel(NameValuePair pair, Func<string, bool> nameValidator, params IValidatable[] dependencies) : this(pair, nameValidator, _ => true, dependencies) { }

    public NameValuePairModel(NameValuePair pair, Func<string, bool> nameValidator, Func<string, bool> valueValidator, params IValidatable[] dependencies) {
        Name = new(() => pair.Name, value => pair.Name = value, nameValidator, dependencies);
        Value = new(() => pair.Value, value => pair.Value = value, valueValidator, dependencies);

        ConfigureState([Name, Value]);
    }

    public void Reset() {
        Name.Reset();
        Value.Reset();
    }
}
