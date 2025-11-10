using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace RequestFiend.UI.Models;

public partial class MainPageModel : ObservableObject {
    private List<RecentCollectionModel> recentCollections = Configuration.RecentCollections.Get();

    public NewRequestTemplateCollectionModel NewRequestTemplateCollection { get; set; } = new();
    public List<RecentCollectionModel> RecentCollections { 
        get => recentCollections;
        set => SetProperty(ref recentCollections, value);
    }
}
