using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Core;

public interface IPasswordProvider {
    bool CanProvide(ISecretOwner owner);
    bool TryProvide(ISecretOwner owner, [NotNullWhen(true)] out string? password);
}
