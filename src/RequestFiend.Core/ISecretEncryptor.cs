using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface ISecretEncryptor {
    Task<string?> Encrypt(ISecretOwner owner, string decryptedValue);
    Task<string?> Decrypt(ISecretOwner owner, string encryptedValue);
}
