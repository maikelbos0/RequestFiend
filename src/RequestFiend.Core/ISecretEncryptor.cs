namespace RequestFiend.Core;

public interface ISecretEncryptor {
    string? Encrypt(ISecretOwner owner, string decryptedValue);
    string? Decrypt(ISecretOwner owner, string encryptedValue);
}
