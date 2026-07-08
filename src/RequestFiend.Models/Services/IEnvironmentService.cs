using RequestFiend.Core;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IEnvironmentService {
    Task<Environment> GetActiveEnvironment();
    Task Save(string filePath, Environment environment);
}
