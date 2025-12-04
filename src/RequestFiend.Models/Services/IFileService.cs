using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IFileService {
    Task WriteAllTextAsync(string filePath, string contents);
}
