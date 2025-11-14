using Microsoft.Maui.Storage;
using RequestFiend.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RequestFiend.UI.Configuration;

public static class RecentCollections {
    // TODO create user preferences for max and for disabling and clearing
    private const int MaximumItemCount = 10;

    public static List<RecentCollectionModel> Get()
        => JsonSerializer.Deserialize<List<RecentCollectionModel>>(Preferences.Get(nameof(RecentCollections), "[]")) ?? [];

    public static void Set(List<RecentCollectionModel> recentCollections)
        => Preferences.Set(nameof(RecentCollections), JsonSerializer.Serialize(recentCollections));

    public static List<RecentCollectionModel> Push(string filePath) {
        var recentCollections = Get();

        recentCollections.RemoveAll(recentCollection => string.Equals(recentCollection.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));
        recentCollections.Insert(0, new(filePath));

        if (recentCollections.Count > MaximumItemCount) {
            recentCollections.RemoveRange(MaximumItemCount, recentCollections.Count - MaximumItemCount);
        }

        Set(recentCollections);

        return recentCollections;
    }

    public static List<RecentCollectionModel> Remove(string filePath) {
        var recentCollections = Get();

        recentCollections.RemoveAll(recentCollection => string.Equals(recentCollection.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));
        Set(recentCollections);

        return recentCollections;
    }
}
