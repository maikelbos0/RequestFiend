using RequestFiend.Core;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public interface IVariableSnapshotProvider {
    Task<VariableSnapshot> CreateVariableSnapshot();
}
