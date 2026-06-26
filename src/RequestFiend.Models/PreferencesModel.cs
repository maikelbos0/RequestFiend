using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class PreferencesModel : PageBoundModelBase {
    private readonly IPreferencesService preferencesService;
    private readonly IMessageService messageService;
    private readonly IPopupService popupService;

    public ValidatableProperty<string> MaximumRecentCollectionCount { get; }
    public ValidatableProperty<string> ScriptEvaluationMode { get; }
    public ValidatableProperty<string> RequestTimeoutInSeconds { get; }
    public ValidatableProperty<string> ExchangeLoggingPath { get; }
    public ValidatableProperty<string> ExchangeLoggingOutputTemplate { get; }
    [ObservableProperty] public partial FileModelCollection Environments { get; private set; }
    public ValidatableProperty<FileModel?> ActiveEnvironment { get; }

    public PreferencesModel(IPreferencesService preferencesService, IMessageService messageService, IPopupService popupService) : base("Preferences", "Preferences") {
        this.preferencesService = preferencesService;
        this.messageService = messageService;
        this.popupService = popupService;

        MaximumRecentCollectionCount = new(() => preferencesService.GetMaximumRecentCollectionCount().ToString(), Validator.Numeric);
        ScriptEvaluationMode = new(() => Options.ScriptEvaluationModeMap[preferencesService.GetScriptEvaluationMode()]);
        RequestTimeoutInSeconds = new(() => preferencesService.GetRequestTimeoutInSeconds()?.ToString() ?? "", Validator.Numeric);
        ExchangeLoggingPath = new(preferencesService.GetExchangeLoggingPath);
        ExchangeLoggingOutputTemplate = new(preferencesService.GetExchangeLoggingOutputTemplate);
        Environments = new(preferencesService.GetEnvironments());
        ActiveEnvironment = new(preferencesService.GetActiveEnvironment);

        ConfigureState([RequestTimeoutInSeconds, MaximumRecentCollectionCount, ScriptEvaluationMode, ExchangeLoggingPath, ExchangeLoggingOutputTemplate, Environments, ActiveEnvironment]);
    }

    [RelayCommand]
    public void Update() {
        if (HasError) {
            return;
        }

        preferencesService.SetMaximumRecentCollectionCount(int.Parse("0" + MaximumRecentCollectionCount.Value));
        preferencesService.SetScriptEvaluationMode(GetScriptEvaluationMode());
        preferencesService.SetRequestTimeoutInSeconds(RequestTimeoutInSeconds.Value.Length == 0 ? null : int.Parse(RequestTimeoutInSeconds.Value));
        preferencesService.SetExchangeLoggingPath(ExchangeLoggingPath.Value);
        preferencesService.SetExchangeLoggingOutputTemplate(ExchangeLoggingOutputTemplate.Value);
        preferencesService.SetEnvironments(Environments);
        preferencesService.SetActiveEnvironment(ActiveEnvironment.Value);

        MaximumRecentCollectionCount.Reset();
        ScriptEvaluationMode.Reset();
        RequestTimeoutInSeconds.Reset();
        ExchangeLoggingPath.Reset();
        ExchangeLoggingOutputTemplate.Reset();
        Environments = new(preferencesService.GetEnvironments());
        ActiveEnvironment.Reset();

        preferencesService.TrimRecentCollections();

        messageService.Send(new PreferencesUpdatedMessage());
        messageService.Send(new SuccessMessage("Preferences have been updated"));
    }

    private ScriptEvaluationMode GetScriptEvaluationMode() {
        if (Options.ReverseScriptEvaluationModeMap.TryGetValue(ScriptEvaluationMode.Value, out var scriptEvaluationMode)) {
            return scriptEvaluationMode;
        }

        return Models.ScriptEvaluationMode.Disabled;
    }


    /*public List<FileModel> AddEnvironment(string filePath) {
        var environments = GetEnvironments();

        environments.Add(new(filePath));
        environments.Sort((x, y) => StringComparer.InvariantCultureIgnoreCase.Compare(x.Name, y.Name));

        Preferences.Set(Environments, JsonSerializer.Serialize(environments));
        return environments;
    }

    public List<FileModel> RemoveEnvironment(FileModel file) {
        var environments = GetEnvironments();

        environments.Remove(file);

        Preferences.Set(Environments, JsonSerializer.Serialize(environments));
        return environments;
    }
*/

    [RelayCommand]
    public async Task Reset() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to reset your preferences?")) {
            preferencesService.Reset();
            MaximumRecentCollectionCount.Reset();
            ScriptEvaluationMode.Reset();
            RequestTimeoutInSeconds.Reset();
            ExchangeLoggingPath.Reset();
            ExchangeLoggingOutputTemplate.Reset();
            Environments = new(preferencesService.GetEnvironments());
            ActiveEnvironment.Reset();

            messageService.Send(new PreferencesUpdatedMessage());
            messageService.Send(new SuccessMessage("Preferences have been reset"));
        }
    }

    [RelayCommand]
    public async Task ClearRecentCollections() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to clear your recent collections?")) {
            preferencesService.ClearRecentCollections();

            messageService.Send(new SuccessMessage("Recent collections have been cleared"));
        }
    }
}
