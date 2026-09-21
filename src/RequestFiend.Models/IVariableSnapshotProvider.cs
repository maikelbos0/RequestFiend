using RequestFiend.Core;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public interface IVariableSnapshotProvider {
    FileModel File { get; }
    Task<VariableSnapshot> CreateVariableSnapshot();
}
