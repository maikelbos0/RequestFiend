using System.IO;

namespace RequestFiend.UI.Models;

public record RecentCollectionModel(string FilePath) {
    public string DisplayName { get; } = Path.GetFileName(FilePath);
};
