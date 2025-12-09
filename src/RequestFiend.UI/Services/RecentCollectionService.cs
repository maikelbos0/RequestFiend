using Microsoft.Maui.Storage;
using RequestFiend.Models;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RequestFiend.UI.Services;

public class RecentCollectionService : IRecentCollectionService {
    // TODO create user preferences for max and for disabling and clearing
    private const int MaximumItemCount = 10;
    private const string RecentCollections = nameof(RecentCollections);

    private readonly IMessageService messageService;

    public RecentCollectionService(IMessageService messageService) {
        this.messageService = messageService;
    }

    public List<RecentCollectionModel> Get()
        => JsonSerializer.Deserialize<List<RecentCollectionModel>>(Preferences.Get(RecentCollections, "[]")) ?? [];

    public List<RecentCollectionModel> Push(string filePath) {
        var recentCollections = Get();

        recentCollections.RemoveAll(recentCollection => string.Equals(recentCollection.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));
        recentCollections.Insert(0, new(filePath));

        if (recentCollections.Count > MaximumItemCount) {
            recentCollections.RemoveRange(MaximumItemCount, recentCollections.Count - MaximumItemCount);
        }

        Set(recentCollections);

        return recentCollections;
    }

    public List<RecentCollectionModel> Remove(string filePath) {
        var recentCollections = Get();

        recentCollections.RemoveAll(recentCollection => string.Equals(recentCollection.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));
        Set(recentCollections);

        return recentCollections;
    }

    private void Set(List<RecentCollectionModel> recentCollections) {
        Preferences.Set(nameof(RecentCollections), JsonSerializer.Serialize(recentCollections));
        messageService.Send(new RecentCollectionsChangedMessage());
    }
}
