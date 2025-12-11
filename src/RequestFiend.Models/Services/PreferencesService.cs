using Microsoft.Maui.Storage;
using RequestFiend.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace RequestFiend.Models.Services;

public class PreferencesService : IPreferencesService {
    public const bool DefaultSaveRecentCollections = true;
    private const int DefaultMaximumRecentCollectionCount = 10;
    private const string SaveRecentCollections = nameof(SaveRecentCollections);
    private const string RecentCollections = nameof(RecentCollections);
    private const string MaximumRecentCollectionCount = nameof(MaximumRecentCollectionCount);

    private readonly IMessageService messageService;

    public PreferencesService(IMessageService messageService) {
        this.messageService = messageService;
    }

    public bool GetSaveRecentCollections()
        => Preferences.Get(SaveRecentCollections, DefaultSaveRecentCollections);
    public void SetSaveRecentCollections(bool saveRecentCollections)
        => Preferences.Set(SaveRecentCollections, saveRecentCollections);

    public int GetMaximumRecentCollectionCount()
        => Preferences.Get(MaximumRecentCollectionCount, DefaultMaximumRecentCollectionCount);

    public void SetMaximumRecentCollectionCount(int maximumRecentCollectionCount)
        => Preferences.Set(MaximumRecentCollectionCount, maximumRecentCollectionCount);

    public List<RecentCollectionModel> GetRecentCollections()
        => JsonSerializer.Deserialize<List<RecentCollectionModel>>(Preferences.Get(RecentCollections, "[]")) ?? [];

    public void TrimRecentCollections()
        => SetRecentCollections([.. GetRecentCollections().Take(GetMaximumRecentCollectionCount())]);

    private void SetRecentCollections(List<RecentCollectionModel> recentCollections) {
        Preferences.Set(RecentCollections, JsonSerializer.Serialize(recentCollections));
        messageService.Send(new RecentCollectionsChangedMessage());
    }

    public void ClearRecentCollections() {
        Preferences.Remove(RecentCollections);
        messageService.Send(new RecentCollectionsChangedMessage());
    }

    public void PushRecentCollection(string filePath) {
        if (!GetSaveRecentCollections()) {
            return;
        }

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
        if (!GetSaveRecentCollections()) {
            return;
        }

        var recentCollections = GetRecentCollections();

        recentCollections.RemoveAll(recentCollection => string.Equals(recentCollection.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));

        SetRecentCollections(recentCollections);
    }

    public void Reset() {
        Preferences.Clear();
        messageService.Send(new RecentCollectionsChangedMessage());
    }
}
