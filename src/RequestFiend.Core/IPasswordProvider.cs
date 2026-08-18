using System;

namespace RequestFiend.Core;

public interface IPasswordProvider {
    bool CanProvide(ISecretOwner owner);
    bool TryProvide(ISecretOwner owner, out ReadOnlyMemory<byte> password);
}
