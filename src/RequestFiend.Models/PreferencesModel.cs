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

    // TODO make it 2 different settings
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

    // TODO add confirmation
    [RelayCommand]
    public void Reset() => preferencesService.Reset();
}
