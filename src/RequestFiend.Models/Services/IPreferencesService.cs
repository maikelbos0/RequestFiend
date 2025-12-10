using System.Collections.Generic;

namespace RequestFiend.Models.Services;

public interface IPreferencesService {
    int GetMaximumRecentCollectionCount();
    void SetMaximumRecentCollectionCount(int maximumRecentCollectionCount);
    List<RecentCollectionModel> GetRecentCollections();
    void SetRecentCollections(List<RecentCollectionModel> recentCollections);
    void PushRecentCollection(string filePath);
    void RemoveRecentCollection(string filePath);
    void Reset();
}
