using System.IO;

namespace RequestFiend.Models;

public record FileModel(string FilePath) : IImmutable {
    public string Name { get; } = Path.GetFileNameWithoutExtension(FilePath);
}
