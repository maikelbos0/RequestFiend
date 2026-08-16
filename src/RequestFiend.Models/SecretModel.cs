using Microsoft.Extensions.DependencyInjection;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class SecretModel : BoundModelBase, IValidatable {
    private readonly ISecretOwner owner;

    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Value { get; }

    public SecretModel(ISecretOwner owner, Secret secret) {
        this.owner = owner;

        Name = new(() => secret.Name, value => secret.Name = value, Validator.Required);
        Value = new(() => secret.GetPlaintextValue(owner), value => secret.SetPlaintextValue(owner, value));

        ConfigureState([Name, Value]);
    }

    public override void Set() {
        if (AppHost.Services.GetRequiredService<IPasswordProvider>().CanProvide(owner)) {
            base.Set();
        }
    }
}
