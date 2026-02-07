using CommunityToolkit.Mvvm.Input;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class PreferencesModel : BoundModelBase {
    private readonly IPreferencesService preferencesService;
    private readonly IMessageService messageService;
    private readonly IPopupService popupService;

    public bool ShowRecentCollections {
        get => field;
        set => SetProperty(ref field, value);
    }

    public ValidatableProperty<string?> MaximumRecentCollectionCount { get; set; }

    public PreferencesModel(IPreferencesService preferencesService, IMessageService messageService, IPopupService popupService) {
        this.preferencesService = preferencesService;
        this.messageService = messageService;
        this.popupService = popupService;

        ShowRecentCollections = preferencesService.GetShowRecentCollections();
        MaximumRecentCollectionCount = new(() => preferencesService.GetMaximumRecentCollectionCount().ToString(), Validator.Numeric);
    }

    [RelayCommand]
    public void Update() {
        if (MaximumRecentCollectionCount.HasError) {
            return;
        }

        preferencesService.SetShowRecentCollections(ShowRecentCollections);
        preferencesService.SetMaximumRecentCollectionCount(int.Parse(MaximumRecentCollectionCount.Value!));

        if (ShowRecentCollections) {
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
            ShowRecentCollections = preferencesService.GetShowRecentCollections();
            MaximumRecentCollectionCount.Reset();

            messageService.Send(new SuccessMessage("Preferences have been reset"));
        }
    }
}
