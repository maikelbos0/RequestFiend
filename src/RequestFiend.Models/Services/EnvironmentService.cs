using RequestFiend.Core;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public class EnvironmentService : IEnvironmentService {
    private readonly IFileSystem fileSystem;

    public EnvironmentService(IFileSystem fileSystem) {
        this.fileSystem = fileSystem;
    }

    public Task Save(string filePath, Environment environment)
        => fileSystem.File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(environment));
}
