using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class EnvironmentModel : PageBoundModelBase {
    private readonly System.Func<CancellationToken, Task> closeMethod;
    private readonly IEnvironmentService environmentService;
    private readonly Environment environment;

    public FileModel File { get; }
    public NameValuePairModelCollection Variables { get; }

    public EnvironmentModel(System.Func<CancellationToken, Task> closeMethod, IEnvironmentService environmentService, FileModel file, Environment environment) : base(file.Name, file.Name) {
        this.closeMethod = closeMethod;
        this.environmentService = environmentService;
        File = file;
        this.environment = environment;
        Variables = new(environment.Variables, Validator.VariableName);

        ConfigureState([Variables]);
    }

    [RelayCommand]
    public async Task Update(CancellationToken cancellationToken) {
        if (HasError) {
            return;
        }

        await environmentService.Save(File.FilePath, environment);
        await closeMethod(cancellationToken);
    }
}
