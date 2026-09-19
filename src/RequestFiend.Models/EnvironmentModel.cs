using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class EnvironmentModel : BoundModelBase {
    private readonly System.Func<CancellationToken, Task> closeMethod;
    private readonly IEnvironmentService environmentService;
    private readonly IPopupService popupService;
    private readonly ISecretEncryptor secretEncryptor;
    private readonly FileModel file;
    private readonly Environment environment;

    public NameValuePairModelCollection Variables { get; }
    public SecretModelCollection Secrets { get; }
    [ObservableProperty] public partial bool IsLocked { get; set; } = true;

    public EnvironmentModel(
        System.Func<CancellationToken, Task> closeMethod,
        IEnvironmentService environmentService,
        IPopupService popupService,
        ISecretEncryptor secretEncryptor,
        FileModel file,
        Environment environment
    ) {
        this.closeMethod = closeMethod;
        this.environmentService = environmentService;
        this.popupService = popupService;
        this.secretEncryptor = secretEncryptor;
        this.file = file;
        this.environment = environment;

        Variables = new(environment.Variables, Validator.VariableName);
        Secrets = new(environment, environment.Secrets);

        ConfigureState([Variables, Secrets]);
    }

    [RelayCommand]
    public async Task Update(CancellationToken cancellationToken) {
        if (HasError) {
            return;
        }

        Set();

        await environmentService.Save(file, environment);
        await closeMethod(cancellationToken);
    }

    [RelayCommand]
    public async Task Unlock() {
        var result = await popupService.ShowUnlockPopup(secretEncryptor, environment);
        IsLocked = !result.Result;
    }

    [RelayCommand]
    public void Lock() {
        secretEncryptor.Lock(environment);
        IsLocked = true;
    }
}
