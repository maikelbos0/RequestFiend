using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface IPasswordProvider {
    Task<string?> Provide(ISecretOwner owner);
}
