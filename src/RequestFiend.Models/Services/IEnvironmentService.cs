using RequestFiend.Core;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IEnvironmentService {
    Task<Environment> GetActiveEnvironment();
    Task Save(FileModel file, Environment environment);
}
