using System.Collections.Generic;

namespace RequestFiend.Models.Services;

public interface IPreferencesService {
    bool GetShowRecentCollections();
    void SetShowRecentCollections(bool saveRecentCollections);
    int GetMaximumRecentCollectionCount();
    void SetMaximumRecentCollectionCount(int maximumRecentCollectionCount);
    List<RequestTemplateCollectionFileModel> GetRecentCollections();
    void TrimRecentCollections();
    void ClearRecentCollections();
    void PushRecentCollection(string filePath);
    void RemoveRecentCollection(string filePath);
    void Reset();
    bool GetAllowScriptExecution();
    void SetAllowScriptExecution(bool allowScriptExecution);
}
