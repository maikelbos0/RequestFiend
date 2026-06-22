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

    public PreferencesModel(IPreferencesService preferencesService, IMessageService messageService, IPopupService popupService) : base("Preferences", "Preferences") {
        this.preferencesService = preferencesService;
        this.messageService = messageService;
        this.popupService = popupService;

        MaximumRecentCollectionCount = new(() => preferencesService.GetMaximumRecentCollectionCount().ToString(), Validator.Numeric);
        ScriptEvaluationMode = new(() => Options.ScriptEvaluationModeMap[preferencesService.GetScriptEvaluationMode()]);
        RequestTimeoutInSeconds = new(() => preferencesService.GetRequestTimeoutInSeconds()?.ToString() ?? "", Validator.Numeric);
        ExchangeLoggingPath = new(preferencesService.GetExchangeLoggingPath);
        ExchangeLoggingOutputTemplate = new(preferencesService.GetExchangeLoggingOutputTemplate);

        ConfigureState([RequestTimeoutInSeconds, MaximumRecentCollectionCount, ScriptEvaluationMode, ExchangeLoggingPath, ExchangeLoggingOutputTemplate]);
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

        MaximumRecentCollectionCount.Reset();
        ScriptEvaluationMode.Reset();
        RequestTimeoutInSeconds.Reset();
        ExchangeLoggingPath.Reset();
        ExchangeLoggingOutputTemplate.Reset();

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

    [RelayCommand]
    public async Task Reset() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to reset your preferences?")) {
            preferencesService.Reset();
            MaximumRecentCollectionCount.Reset();
            ScriptEvaluationMode.Reset();
            RequestTimeoutInSeconds.Reset();
            ExchangeLoggingPath.Reset();
            ExchangeLoggingOutputTemplate.Reset();

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
