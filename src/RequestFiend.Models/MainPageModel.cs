using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace RequestFiend.Models;

public partial class MainPageModel : ObservableObject {
    private List<RecentCollectionModel> recentCollections = [];

    public List<RecentCollectionModel> RecentCollections { 
        get => recentCollections;
        set => SetProperty(ref recentCollections, value);
    }
}
