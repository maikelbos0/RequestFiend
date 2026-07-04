using RequestFiend.Core;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IEnvironmentService {
    Task Save(string filePath, Environment environment);
}
