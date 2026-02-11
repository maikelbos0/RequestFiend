using System.IO;

namespace RequestFiend.Models;

public record RequestTemplateCollectionFileModel(string FilePath) {
    public string Name { get; } = Path.GetFileNameWithoutExtension(FilePath);
};
