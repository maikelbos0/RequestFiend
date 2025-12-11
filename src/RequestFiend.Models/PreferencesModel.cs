using CommunityToolkit.Mvvm.Input;
using RequestFiend.Models.Services;
using System;
using System.Linq;

namespace RequestFiend.Models;

public partial class PreferencesModel : BoundModelBase {
    private readonly IPreferencesService preferencesService;

    // TODO add custom entry or something
    public int MaximumRecentCollectionCount {
        get => field;
        set => SetProperty(ref field, Math.Max(value, 0));
    }

    public bool SaveRecentCollections {
        get => field;
        set => SetProperty(ref field, value);
    }

    public PreferencesModel(IPreferencesService preferencesService) {
        this.preferencesService = preferencesService;

        SaveRecentCollections = preferencesService.GetSaveRecentCollections();
        MaximumRecentCollectionCount = preferencesService.GetMaximumRecentCollectionCount();
        SaveRecentCollections = MaximumRecentCollectionCount > 0;
    }

    [RelayCommand]
    public void Update() {
        preferencesService.SetSaveRecentCollections(SaveRecentCollections);
        preferencesService.SetMaximumRecentCollectionCount(MaximumRecentCollectionCount);

        if (SaveRecentCollections) {
            preferencesService.TrimRecentCollections();
        }
        else {
            preferencesService.ClearRecentCollections();
        }
    }

    // TODO add confirmation
    [RelayCommand]
    public void Reset() => preferencesService.Reset();
}
