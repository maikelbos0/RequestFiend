using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class EnvironmentModel : PageBoundModelBase {
    private readonly IEnvironmentService environmentService;
    private readonly IMessageService messageService;
    private readonly Environment environment;

    public FileModel File { get; }
    public NameValuePairModelCollection Variables { get; }

    public EnvironmentModel(IEnvironmentService environmentService, IMessageService messageService, FileModel file, Environment environment) : base(file.Name, file.Name) {
        this.environmentService = environmentService;
        this.messageService = messageService;
        File = file;
        this.environment = environment;
        Variables = new(environment.Variables, Validator.VariableName);

        ConfigureState([Variables]);
    }

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        Reset();

        await environmentService.Save(File.FilePath, environment);
        messageService.Send(new SuccessMessage("Changes have been saved"));
    }
}
