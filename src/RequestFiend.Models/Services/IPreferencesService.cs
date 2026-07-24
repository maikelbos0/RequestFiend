using Serilog.Events;
using System.Collections.Generic;

namespace RequestFiend.Models.Services;

public interface IPreferencesService {
    int GetMaximumRecentCollectionCount();
    void SetMaximumRecentCollectionCount(int maximumRecentCollectionCount);
    List<FileModel> GetRecentCollections();
    void TrimRecentCollections();
    void ClearRecentCollections();
    void PushRecentCollection(string filePath);
    void RemoveRecentCollection(string filePath);
    ScriptEvaluationMode GetScriptEvaluationMode();
    void SetScriptEvaluationMode(ScriptEvaluationMode scriptEvaluationMode);
    bool GetCollectionAllowScriptEvaluation(string filePath);
    void SetCollectionAllowScriptEvaluation(string filePath, bool allowScriptEvaluation);
    int? GetRequestTimeoutInSeconds();
    void SetRequestTimeoutInSeconds(int? requestTimeoutInSeconds);
    string GetLoggingPath();
    void SetLoggingPath(string loggingPath);
    string GetLoggingOutputTemplate();
    void SetLoggingOutputTemplate(string loggingOutputTemplate);
    LogEventLevel GetMinimumExchangeLoggingLevel();
    void SetMinimumExchangeLoggingLevel(LogEventLevel minimumExchangeLoggingLevel);
    LogEventLevel GetMinimumOtherSourceLoggingLevel();
    void SetMinimumOtherSourceLoggingLevel(LogEventLevel minimumOtherSourceLoggingLevel);
    List<FileModel> GetEnvironments();
    void SetEnvironments(IEnumerable<FileModel> environments);
    FileModel? GetActiveEnvironment();
    void SetActiveEnvironment(FileModel? activeEnvironment);
    void Reset();
}
