using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Models.Services;
using System;
using System.Linq;

namespace RequestFiend.Models;

public partial class PreferencesModel : ObservableObject {
    private readonly IPreferencesService preferencesService;

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

        MaximumRecentCollectionCount = preferencesService.GetMaximumRecentCollectionCount();
        SaveRecentCollections = MaximumRecentCollectionCount > 0;
    }

    [RelayCommand]
    public void Update() {
        var maximumRecentCollectionCount = SaveRecentCollections ? MaximumRecentCollectionCount : 0;
            
        preferencesService.SetMaximumRecentCollectionCount(maximumRecentCollectionCount);
        preferencesService.SetRecentCollections(preferencesService.GetRecentCollections().Take(maximumRecentCollectionCount).ToList());
    }

    [RelayCommand]
    public void Reset() => preferencesService.Reset();
}
