using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RequestFiend.Core;

public class SecretEncryptor : ISecretEncryptor {
    public const byte VerificationCode = 42;
    public const int BlockSizeInBytes = 16;
    public const int NonceSizeInBytes = 12;

    private readonly Dictionary<ISecretOwner, byte[]> keyStore = [];
    private readonly Lock keyStoreLock = new();
    private bool isDisposed;

    public bool TryUnlock(ISecretOwner owner, string password) {
        const int Iterations = 1_000_000;

        ObjectDisposedException.ThrowIf(isDisposed, this);

        byte[] key;

        if (owner.EncryptionData == null) {
            var salt = RandomNumberGenerator.GetBytes(BlockSizeInBytes);

            key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, SHA256.HashSizeInBytes);
            byte[] encryptionData = [.. salt, .. Encrypt(key, [VerificationCode])];
            owner.EncryptionData = Convert.ToBase64String(encryptionData);
        }
        else {
            var source = Convert.FromBase64String(owner.EncryptionData);
            var salt = source.AsSpan(0, BlockSizeInBytes);

            key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, SHA256.HashSizeInBytes);

            try {
                _ = Decrypt(key, source.AsSpan(BlockSizeInBytes));
            }
            catch {
                return false;
            }
        }

        lock (keyStoreLock) {
            if (keyStore.Remove(owner, out var previousKey)) {
                CryptographicOperations.ZeroMemory(previousKey);
            }

            keyStore[owner] = key;
        }

        return true;
    }

    public void Lock(ISecretOwner owner) {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        if (keyStore.Remove(owner, out var previousKey)) {
            CryptographicOperations.ZeroMemory(previousKey);
        }
    }

    public bool IsLocked(ISecretOwner owner) {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        return !keyStore.ContainsKey(owner);
    }

    public bool TryEncrypt(ISecretOwner owner, string plaintextValue, [NotNullWhen(true)] out string? result) {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        if (!TryGetKey(owner, out var key)) {
            result = null;
            return false;
        }

        result = Convert.ToBase64String(Encrypt(key, Encoding.UTF8.GetBytes(plaintextValue)));
        return true;
    }

    private static byte[] Encrypt(byte[] key, byte[] plaintext) {
        using var aes = new AesGcm(key, BlockSizeInBytes);

        var target = new byte[NonceSizeInBytes + BlockSizeInBytes + plaintext.Length];
        var nonce = target.AsSpan(0, NonceSizeInBytes);
        var tag = target.AsSpan(NonceSizeInBytes, BlockSizeInBytes);
        var ciphertext = target.AsSpan(NonceSizeInBytes + BlockSizeInBytes);

        Buffer.BlockCopy(RandomNumberGenerator.GetBytes(BlockSizeInBytes), 0, target, 0, BlockSizeInBytes);

        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        return target;
    }

    public bool TryDecrypt(ISecretOwner owner, string encryptedValue, [NotNullWhen(true)] out string? result) {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        if (!TryGetKey(owner, out var key)) {
            result = null;
            return false;
        }

        result = Encoding.UTF8.GetString(Decrypt(key, Convert.FromBase64String(encryptedValue)));
        return true;
    }

    private static byte[] Decrypt(byte[] key, Span<byte> source) {
        using var aes = new AesGcm(key, BlockSizeInBytes);

        var nonce = source[..NonceSizeInBytes];
        var tag = source.Slice(NonceSizeInBytes, BlockSizeInBytes);
        var ciphertext = source[(NonceSizeInBytes + BlockSizeInBytes)..];
        var plaintext = new byte[ciphertext.Length];

        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return plaintext;
    }

    private bool TryGetKey(ISecretOwner owner, [NotNullWhen(true)] out byte[]? key) {
        lock (keyStoreLock) {
            return keyStore.TryGetValue(owner, out key);
        }
    }

    public void Dispose() {
        if (isDisposed) {
            return;
        }

        isDisposed = true;

        lock (keyStoreLock) {
            foreach (var key in keyStore.Values) {
                CryptographicOperations.ZeroMemory(key);
            }
        }
    }
}
