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
    private readonly FileModel file;
    private readonly Environment environment;

    public NameValuePairModelCollection Variables { get; }

    public EnvironmentModel(System.Func<CancellationToken, Task> closeMethod, IEnvironmentService environmentService, FileModel file, Environment environment) {
        this.closeMethod = closeMethod;
        this.environmentService = environmentService;
        this.file = file;
        this.environment = environment;

        Variables = new(environment.Variables, Validator.VariableName);

        ConfigureState([Variables]);
    }

    [RelayCommand]
    public async Task Update(CancellationToken cancellationToken) {
        if (HasError) {
            return;
        }

        await environmentService.Save(file.FilePath, environment);
        await closeMethod(cancellationToken);
    }
}
