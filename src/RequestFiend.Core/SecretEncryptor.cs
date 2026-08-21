using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RequestFiend.Core;

public class SecretEncryptor : ISecretEncryptor {
    public const int BlockSizeInBytes = 16;
    public const int NonceSizeInBytes = 12;

    private readonly Dictionary<ISecretOwner, byte[]> keyStore = [];
    private readonly Lock keyStoreLock = new();
    private bool isDisposed;

    public void Unlock(ISecretOwner owner, string password) {
        const int Iterations = 1_000_000;

        ObjectDisposedException.ThrowIf(isDisposed, this);

        owner.Salt ??= RandomNumberGenerator.GetBytes(BlockSizeInBytes);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, owner.Salt, Iterations, HashAlgorithmName.SHA256, SHA256.HashSizeInBytes);

        lock (keyStoreLock) {
            if (keyStore.Remove(owner, out var previousKey)) {
                CryptographicOperations.ZeroMemory(previousKey);
            }

            keyStore[owner] = key;
        }
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

        using var aes = new AesGcm(key, BlockSizeInBytes);

        var plaintext = Encoding.UTF8.GetBytes(plaintextValue);
        var target = new byte[NonceSizeInBytes + BlockSizeInBytes + plaintext.Length];
        var nonce = target.AsSpan(0, NonceSizeInBytes);
        var tag = target.AsSpan(NonceSizeInBytes, BlockSizeInBytes);
        var ciphertext = target.AsSpan(NonceSizeInBytes + BlockSizeInBytes);

        Buffer.BlockCopy(RandomNumberGenerator.GetBytes(BlockSizeInBytes), 0, target, 0, BlockSizeInBytes);

        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        result = Convert.ToBase64String(target);
        return true;
    }

    public bool TryDecrypt(ISecretOwner owner, string encryptedValue, [NotNullWhen(true)] out string? result) {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        if (!TryGetKey(owner, out var key)) {
            result = null;
            return false;
        }

        using var aes = new AesGcm(key, BlockSizeInBytes);

        var source = Convert.FromBase64String(encryptedValue);
        var nonce = source.AsSpan(0, NonceSizeInBytes);
        var tag = source.AsSpan(NonceSizeInBytes, BlockSizeInBytes);
        var ciphertext = source.AsSpan(NonceSizeInBytes + BlockSizeInBytes);
        var plaintext = new byte[ciphertext.Length];

        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        result = Encoding.UTF8.GetString(plaintext);
        return true;
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
