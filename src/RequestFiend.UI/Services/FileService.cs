using RequestFiend.Models.Services;
using System.IO;
using System.Threading.Tasks;

namespace RequestFiend.UI.Services;

public class FileService : IFileService {
    public Task WriteAllTextAsync(string filePath, string contents)
        
        => File.WriteAllTextAsync(filePath, contents);
}
