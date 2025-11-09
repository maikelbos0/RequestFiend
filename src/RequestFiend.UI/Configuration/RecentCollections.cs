using Microsoft.Maui.Storage;
using RequestFiend.UI.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RequestFiend.UI.Configuration;

public static class RecentCollections {
    // TODO create user preferences for max and for disabling and clearing
    private const int MaximumItemCount = 10;

    public static List<RecentCollectionModel> Add(string filePath, string name) {
        var recentCollections = Get();

        recentCollections.RemoveAll(x => string.Equals(x.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));
        recentCollections.Insert(0, new(filePath, name));

        if (recentCollections.Count > MaximumItemCount) {
            recentCollections.RemoveRange(MaximumItemCount, recentCollections.Count - MaximumItemCount);
        }

        Preferences.Set(nameof(RecentCollections), JsonSerializer.Serialize(recentCollections));

        return recentCollections;
    }

    public static List<RecentCollectionModel> Remove(string filePath) {
        var recentCollections = Get();

        recentCollections.RemoveAll(x => string.Equals(x.FilePath, filePath, StringComparison.InvariantCultureIgnoreCase));
        Preferences.Set(nameof(RecentCollections), JsonSerializer.Serialize(recentCollections));

        return recentCollections;
    }

    public static List<RecentCollectionModel> Get()
        => JsonSerializer.Deserialize<List<RecentCollectionModel>>(Preferences.Get(nameof(RecentCollections), "[]")) ?? [];
}
