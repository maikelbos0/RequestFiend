using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Core;

public interface ISecretEncryptor {
    bool TryEncrypt(ISecretOwner owner, string plaintextValue, [NotNullWhen(true)] out string? result);
    bool TryDecrypt(ISecretOwner owner, string encryptedValue, [NotNullWhen(true)] out string? result);
}
