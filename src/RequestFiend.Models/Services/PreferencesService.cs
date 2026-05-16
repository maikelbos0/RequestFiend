using Microsoft.Maui.Storage;
using RequestFiend.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace RequestFiend.Models.Services;

public class PreferencesService : IPreferencesService {
    private const int DefaultMaximumRecentCollectionCount = 10;
    private const int DefaultScriptEvaluationMode = (int)ScriptEvaluationMode.Disabled;
    private const bool DefaultCollectionAllowScriptEvaluation = false;
    private const int InfiniteRequestTimeoutInSeconds = -1;
    private const int DefaultRequestTimeoutInSeconds = InfiniteRequestTimeoutInSeconds;
    private const string ShowRecentCollections = nameof(ShowRecentCollections);
    private const string RecentCollections = nameof(RecentCollections);
    private const string MaximumRecentCollectionCount = nameof(MaximumRecentCollectionCount);
    private const string CollectionAllowScriptEvaluation = nameof(CollectionAllowScriptEvaluation);
    private const string RequestTimeoutInSeconds = nameof(RequestTimeoutInSeconds);

    private readonly IMessageService messageService;

    public PreferencesService(IMessageService messageService) {
        this.messageService = messageService;
    }

    public int GetMaximumRecentCollectionCount()
        => Preferences.Get(MaximumRecentCollectionCount, DefaultMaximumRecentCollectionCount);

    public void SetMaximumRecentCollectionCount(int maximumRecentCollectionCount)
        => Preferences.Set(MaximumRecentCollectionCount, maximumRecentCollectionCount);

    public List<RequestTemplateCollectionFileModel> GetRecentCollections()
        => JsonSerializer.Deserialize<List<RequestTemplateCollectionFileModel>>(Preferences.Get(RecentCollections, "[]")) ?? [];

    public void TrimRecentCollections()
        => SetRecentCollections([.. GetRecentCollections().Take(GetMaximumRecentCollectionCount())]);

    private void SetRecentCollections(List<RequestTemplateCollectionFileModel> recentCollections) {
        Preferences.Set(RecentCollections, JsonSerializer.Serialize(recentCollections));
        messageService.Send(new RecentCollectionsChangedMessage());
    }

    public void ClearRecentCollections() {
        Preferences.Remove(RecentCollections);
        messageService.Send(new RecentCollectionsChangedMessage());
    }

    public void PushRecentCollection(string filePath) {
        var recentCollections = GetRecentCollections();
        var maximumRecentCollectionCount = GetMaximumRecentCollectionCount();

        recentCollections.RemoveAll(recentCollection => string.Equals(recentCollection.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));
        recentCollections.Insert(0, new(filePath));

        if (recentCollections.Count > maximumRecentCollectionCount) {
            recentCollections.RemoveRange(maximumRecentCollectionCount, recentCollections.Count - maximumRecentCollectionCount);
        }

        SetRecentCollections(recentCollections);
    }

    public void RemoveRecentCollection(string filePath) {
        var recentCollections = GetRecentCollections();

        recentCollections.RemoveAll(recentCollection => string.Equals(recentCollection.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));

        SetRecentCollections(recentCollections);
    }

    public ScriptEvaluationMode GetScriptEvaluationMode()
        => (ScriptEvaluationMode)Preferences.Get(nameof(ScriptEvaluationMode), DefaultScriptEvaluationMode);

    public void SetScriptEvaluationMode(ScriptEvaluationMode scriptEvaluationMode)
        => Preferences.Set(nameof(ScriptEvaluationMode), (int)scriptEvaluationMode);

    public bool GetCollectionAllowScriptEvaluation(string filePath)
        => Preferences.Get($"{CollectionAllowScriptEvaluation}_{filePath}", DefaultCollectionAllowScriptEvaluation);

    public void SetCollectionAllowScriptEvaluation(string filePath, bool allowScriptEvaluation)
        => Preferences.Set($"{CollectionAllowScriptEvaluation}_{filePath}", allowScriptEvaluation);

    public int? GetRequestTimeoutInSeconds() {
        var requestTimeoutInSeconds = Preferences.Get(RequestTimeoutInSeconds, DefaultRequestTimeoutInSeconds);

        if (requestTimeoutInSeconds == InfiniteRequestTimeoutInSeconds) {
            return null;
        }

        return requestTimeoutInSeconds;
    }

    public void SetRequestTimeoutInSeconds(int? requestTimeoutInSeconds)
        => Preferences.Set(RequestTimeoutInSeconds, requestTimeoutInSeconds ?? InfiniteRequestTimeoutInSeconds);

    public void Reset() {
        Preferences.Clear();
        messageService.Send(new RecentCollectionsChangedMessage());
    }
}
