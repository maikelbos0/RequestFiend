using CommunityToolkit.Mvvm.Input;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;

namespace RequestFiend.Models;

public partial class PreferencesModel : BoundModelBase {
    private readonly IPreferencesService preferencesService;
    private readonly IMessageService messageService;

    // TODO add custom entry or something
    public int MaximumRecentCollectionCount {
        get => field;
        set => SetProperty(ref field, Math.Max(value, 0));
    }

    public bool ShowRecentCollections {
        get => field;
        set => SetProperty(ref field, value);
    }

    public PreferencesModel(IPreferencesService preferencesService, IMessageService messageService) {
        this.preferencesService = preferencesService;
        this.messageService = messageService;
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

    // TODO add confirmation
    [RelayCommand]
    public void Reset() => preferencesService.Reset();
}
