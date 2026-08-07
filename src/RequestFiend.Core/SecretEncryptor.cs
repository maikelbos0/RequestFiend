using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public class SecretEncryptor : ISecretEncryptor {
    public const int BlockSizeInBytes = 16;
    public const int NonceSizeInBytes = 12;

    private readonly IPasswordProvider passwordProvider;

    public SecretEncryptor(IPasswordProvider passwordProvider) {
        this.passwordProvider = passwordProvider;
    }

    public async Task<string?> Encrypt(ISecretOwner owner, string value) {
        var key = await GetKey(owner);

        if (key == null) {
            return null;
        }

        try {
            using var aes = new AesGcm(key, BlockSizeInBytes);

            var plaintext = Encoding.UTF8.GetBytes(value);
            var result = new byte[NonceSizeInBytes + BlockSizeInBytes + plaintext.Length];
            var nonce = result.AsSpan(0, NonceSizeInBytes);
            var tag = result.AsSpan(NonceSizeInBytes, BlockSizeInBytes);
            var ciphertext = result.AsSpan(NonceSizeInBytes + BlockSizeInBytes);

            Buffer.BlockCopy(RandomNumberGenerator.GetBytes(BlockSizeInBytes), 0, result, 0, BlockSizeInBytes);

            aes.Encrypt(nonce, plaintext, ciphertext, tag);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex) {
            return null;
        }
    }

    public async Task<string?> Decrypt(ISecretOwner owner, string encryptedValue) {
        var key = await GetKey(owner);

        if (key == null) {
            return null;
        }

        try {
            using var aes = new AesGcm(key, BlockSizeInBytes);

            var source = Convert.FromBase64String(encryptedValue);
            var nonce = source.AsSpan(0, NonceSizeInBytes);
            var tag = source.AsSpan(NonceSizeInBytes, BlockSizeInBytes);
            var ciphertext = source.AsSpan(NonceSizeInBytes + BlockSizeInBytes);
            var plaintext = new byte[ciphertext.Length];

            aes.Decrypt(nonce, ciphertext, tag, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }
        catch {
            return null;
        }
    }


    private async Task<byte[]?> GetKey(ISecretOwner owner) {
        const int Iterations = 1_000_000;

        var password = await passwordProvider.Provide(owner);

        if (password == null) {
            return null;
        }

        if (owner.Salt == null) {
            owner.Salt = RandomNumberGenerator.GetBytes(BlockSizeInBytes / 8);
        }

        return Rfc2898DeriveBytes.Pbkdf2(password, owner.Salt, Iterations, HashAlgorithmName.SHA256, SHA256.HashSizeInBytes);
    }
}