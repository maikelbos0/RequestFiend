using System.Collections.Generic;

namespace RequestFiend.UI.Models;

public class MainPageModel {
    public NewRequestTemplateCollectionModel NewRequestTemplateCollection { get; set; } = new();
    public List<RecentCollectionModel> RecentCollections { get; set; } = Configuration.RecentCollections.Get();
}
