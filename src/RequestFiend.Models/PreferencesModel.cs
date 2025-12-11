using CommunityToolkit.Mvvm.Input;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class PreferencesModel : BoundModelBase {
    private readonly IPreferencesService preferencesService;
    private readonly IMessageService messageService;
    private readonly IPopupService popupService;

    // TODO add custom entry or something
    public int MaximumRecentCollectionCount {
        get => field;
        set => SetProperty(ref field, Math.Max(value, 0));
    }

    public bool ShowRecentCollections {
        get => field;
        set => SetProperty(ref field, value);
    }

    public PreferencesModel(IPreferencesService preferencesService, IMessageService messageService, IPopupService popupService) {
        this.preferencesService = preferencesService;
        this.messageService = messageService;
        this.popupService = popupService;

        ShowRecentCollections = preferencesService.GetShowRecentCollections();
        MaximumRecentCollectionCount = preferencesService.GetMaximumRecentCollectionCount();
    }

    [RelayCommand]
    public void Update() {
        preferencesService.SetShowRecentCollections(ShowRecentCollections);
        preferencesService.SetMaximumRecentCollectionCount(MaximumRecentCollectionCount);

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
            MaximumRecentCollectionCount = preferencesService.GetMaximumRecentCollectionCount();

            messageService.Send(new SuccessMessage("Preferences have been reset"));
        }
    }
}
