using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Core;

public interface ISecretEncryptor {
    void Unlock(ISecretOwner owner, string password);
    void Lock(ISecretOwner owner);
    bool IsLocked(ISecretOwner owner);
    bool TryEncrypt(ISecretOwner owner, string plaintextValue, [NotNullWhen(true)] out string? result);
    bool TryDecrypt(ISecretOwner owner, string encryptedValue, [NotNullWhen(true)] out string? result);
}
