using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace RequestFiend.Core;

public class SecretEncryptor : ISecretEncryptor {
    public const int BlockSizeInBytes = 16;
    public const int NonceSizeInBytes = 12;

    private readonly IPasswordProvider passwordProvider;

    public SecretEncryptor(IPasswordProvider passwordProvider) {
        this.passwordProvider = passwordProvider;
    }

    public string? Encrypt(ISecretOwner owner, string value) {
        if (!TryGetKey(owner, out var key)) {
            return null;
        }

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

    public string? Decrypt(ISecretOwner owner, string encryptedValue) {
        if (!TryGetKey(owner, out var key)) {
            return null;
        }

        using var aes = new AesGcm(key, BlockSizeInBytes);

        var source = Convert.FromBase64String(encryptedValue);
        var nonce = source.AsSpan(0, NonceSizeInBytes);
        var tag = source.AsSpan(NonceSizeInBytes, BlockSizeInBytes);
        var ciphertext = source.AsSpan(NonceSizeInBytes + BlockSizeInBytes);
        var plaintext = new byte[ciphertext.Length];

        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }

    private bool TryGetKey(ISecretOwner owner, [NotNullWhen(true)] out byte[]? key) {
        const int Iterations = 1_000_000;

        if (!passwordProvider.TryProvide(owner, out var password)) {
            key = null;
            return false;
        }

        if (owner.Salt == null) {
            owner.Salt = RandomNumberGenerator.GetBytes(BlockSizeInBytes / 8);
        }

        key = Rfc2898DeriveBytes.Pbkdf2(password, owner.Salt, Iterations, HashAlgorithmName.SHA256, SHA256.HashSizeInBytes);
        return true;
    }
}