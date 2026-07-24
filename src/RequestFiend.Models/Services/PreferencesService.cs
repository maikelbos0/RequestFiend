using Microsoft.Maui.Storage;
using RequestFiend.Models.Messages;
using Serilog.Events;
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
    private const string DefaultLoggingPath = "";
    private const string DefaultLoggingOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    private const int DefaultExchangeLoggingMinimumLevel = (int)LogEventLevel.Information;
    private const int DefaultOtherSourceLoggingMinimumLevel = (int)LogEventLevel.Information;
    private const string ShowRecentCollections = nameof(ShowRecentCollections);
    private const string RecentCollections = nameof(RecentCollections);
    private const string MaximumRecentCollectionCount = nameof(MaximumRecentCollectionCount);
    private const string CollectionAllowScriptEvaluation = nameof(CollectionAllowScriptEvaluation);
    private const string RequestTimeoutInSeconds = nameof(RequestTimeoutInSeconds);
    private const string LoggingPath = nameof(LoggingPath);
    private const string LoggingOutputTemplate = nameof(LoggingOutputTemplate);
    private const string MinimumExchangeLoggingLevel = nameof(MinimumExchangeLoggingLevel);
    private const string MinimumOtherSourceLoggingLevel = nameof(MinimumOtherSourceLoggingLevel);
    private const string Environments = nameof(Environments);
    private const string ActiveEnvironment = nameof(ActiveEnvironment);

    private readonly IMessageService messageService;

    public PreferencesService(IMessageService messageService) {
        this.messageService = messageService;
    }

    public int GetMaximumRecentCollectionCount()
        => Preferences.Get(MaximumRecentCollectionCount, DefaultMaximumRecentCollectionCount);

    public void SetMaximumRecentCollectionCount(int maximumRecentCollectionCount)
        => Preferences.Set(MaximumRecentCollectionCount, maximumRecentCollectionCount);

    public List<FileModel> GetRecentCollections()
        => JsonSerializer.Deserialize<List<FileModel>>(Preferences.Get(RecentCollections, "[]")) ?? [];

    public void TrimRecentCollections()
        => SetRecentCollections([.. GetRecentCollections().Take(GetMaximumRecentCollectionCount())]);

    private void SetRecentCollections(List<FileModel> recentCollections) {
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

    public string GetLoggingPath()
        => Preferences.Get(LoggingPath, DefaultLoggingPath);

    public void SetLoggingPath(string? loggingPath)
        => Preferences.Set(LoggingPath, loggingPath);

    public string GetLoggingOutputTemplate()
        => Preferences.Get(LoggingOutputTemplate, DefaultLoggingOutputTemplate);

    public void SetLoggingOutputTemplate(string? loggingOutputTemplate)
        => Preferences.Set(LoggingOutputTemplate, loggingOutputTemplate);

    public LogEventLevel GetMinimumExchangeLoggingLevel()
        => (LogEventLevel)Preferences.Get(MinimumExchangeLoggingLevel, DefaultExchangeLoggingMinimumLevel);

    public void SetMinimumExchangeLoggingLevel(LogEventLevel minimumExchangeLoggingLevel)
        => Preferences.Set(MinimumExchangeLoggingLevel, (int)minimumExchangeLoggingLevel);

    public LogEventLevel GetMinimumOtherSourceLoggingLevel()
        => (LogEventLevel)Preferences.Get(MinimumOtherSourceLoggingLevel, DefaultOtherSourceLoggingMinimumLevel);

    public void SetMinimumOtherSourceLoggingLevel(LogEventLevel minimumOtherSourceLoggingLevel)
        => Preferences.Set(MinimumOtherSourceLoggingLevel, (int)minimumOtherSourceLoggingLevel);

    public List<FileModel> GetEnvironments()
        => JsonSerializer.Deserialize<List<FileModel>>(Preferences.Get(Environments, "[]")) ?? [];

    public void SetEnvironments(IEnumerable<FileModel> environments)
        => Preferences.Set(Environments, JsonSerializer.Serialize(environments));

    public FileModel? GetActiveEnvironment()
        => JsonSerializer.Deserialize<FileModel>(Preferences.Get(ActiveEnvironment, "null"));

    public void SetActiveEnvironment(FileModel? activeEnvironment) {
        Preferences.Set(ActiveEnvironment, JsonSerializer.Serialize(activeEnvironment));
        messageService.Send(new ActiveEnvironmentChangedMessage());
    }

    public void Reset() {
        Preferences.Clear();
        messageService.Send(new RecentCollectionsChangedMessage());
    }
}
