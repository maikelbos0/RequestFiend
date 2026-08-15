using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class SecretModel : BoundModelBase, IValidatable {
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Value { get; }

    public SecretModel(ISecretOwner owner, Secret secret) {
        Name = new(() => secret.Name, value => secret.Name = value, Validator.Required);
        Value = new(() => secret.GetValue(owner), value => secret.SetValue(owner, value));

        ConfigureState([Name, Value]);
    }
}
