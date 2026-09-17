using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class UnlockModel : BoundModelBase {
    private readonly Func<CancellationToken, Task> closeMethod;
    private readonly ISecretEncryptor secretEncryptor;
    private readonly ISecretOwner owner;
    
    public ValidatableProperty<string> Password { get; }
    [ObservableProperty] public partial bool IsValid { get; set; }

    public UnlockModel(Func<CancellationToken, Task> closeMethod, ISecretEncryptor secretEncryptor, ISecretOwner owner) {
        this.closeMethod = closeMethod;
        this.secretEncryptor = secretEncryptor;
        this.owner = owner;

        Password = new(() => "", _ => { }, Validator.Required);

        ConfigureState([Password]);
    }

    [RelayCommand]
    public async Task<bool> TryUnlock(CancellationToken cancellationToken) {
        if (HasError) {
            return false;
        }

        IsValid = secretEncryptor.TryUnlock(owner, Password.Value);

        if (IsValid) {
            await closeMethod(cancellationToken);
        }

        return IsValid;
    }
}
