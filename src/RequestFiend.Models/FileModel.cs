using System.IO;

namespace RequestFiend.Models;

public record FileModel(string FilePath) : IImmutable {
    public string Name { get; } = Path.GetFileNameWithoutExtension(FilePath);

    public static implicit operator string(FileModel model) => model.FilePath;

    public static implicit operator FileModel(string filePath) => new(filePath);

    // TODO sorting
    // TODO environment usage comparisons
}
