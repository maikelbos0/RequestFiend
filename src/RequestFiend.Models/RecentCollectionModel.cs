using System.IO;

namespace RequestFiend.Models;

public record RecentCollectionModel(string FilePath) {
    public string Name { get; } = Path.GetFileNameWithoutExtension(FilePath);
};
