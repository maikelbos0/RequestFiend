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

    public ValidatableProperty<bool> ShowRecentCollections { get; }
    public ValidatableProperty<string> MaximumRecentCollectionCount { get; }
    public ValidatableProperty<bool> AllowScriptEvaluation { get; }

    public PreferencesModel(IPreferencesService preferencesService, IMessageService messageService, IPopupService popupService) : base("Preferences", "Preferences") {
        this.preferencesService = preferencesService;
        this.messageService = messageService;
        this.popupService = popupService;

        ShowRecentCollections = new(() => preferencesService.GetShowRecentCollections());
        MaximumRecentCollectionCount = new(() => preferencesService.GetMaximumRecentCollectionCount().ToString(), Validator.Numeric);
        AllowScriptEvaluation = new(() => preferencesService.GetAllowScriptEvaluation());

        ConfigureState([ShowRecentCollections, MaximumRecentCollectionCount, AllowScriptEvaluation]);
    }

    [RelayCommand]
    public void Update() {
        if (HasError) {
            return;
        }

        preferencesService.SetShowRecentCollections(ShowRecentCollections.Value);
        preferencesService.SetMaximumRecentCollectionCount(int.Parse(MaximumRecentCollectionCount.Value!));
        preferencesService.SetAllowScriptEvaluation(AllowScriptEvaluation.Value);

        ShowRecentCollections.Reset();
        MaximumRecentCollectionCount.Reset();
        AllowScriptEvaluation.Reset();

        if (ShowRecentCollections.Value) {
            preferencesService.TrimRecentCollections();
        }
        else {
            preferencesService.ClearRecentCollections();
        }

        messageService.Send(new SuccessMessage("Preferences have been updated"));
    }

    [RelayCommand]
    public async Task Reset() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to reset your preferences?")) {
            preferencesService.Reset();
            ShowRecentCollections.Reset();
            MaximumRecentCollectionCount.Reset();
            AllowScriptEvaluation.Reset();

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

    [RelayCommand]
    public void ToggleShowRecentCollections()
        => ShowRecentCollections.Value = !ShowRecentCollections.Value;

    [RelayCommand]
    public void ToggleAllowScriptEvaluation()
        => AllowScriptEvaluation.Value = !AllowScriptEvaluation.Value;
}
