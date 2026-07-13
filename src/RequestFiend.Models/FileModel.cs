using System.IO;

namespace RequestFiend.Models;

public record FileModel(string FilePath) : NamedItemModel(Path.GetFileNameWithoutExtension(FilePath));

