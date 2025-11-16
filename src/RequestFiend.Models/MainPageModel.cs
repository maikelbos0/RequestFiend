using System.Collections.Generic;

namespace RequestFiend.Models;

public partial class MainPageModel : ModelBase {
    private List<RecentCollectionModel> recentCollections = [];

    public List<RecentCollectionModel> RecentCollections { 
        get => recentCollections;
        set => SetProperty(ref recentCollections, value);
    }
}
