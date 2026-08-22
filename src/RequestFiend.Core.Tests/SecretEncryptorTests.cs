using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Xunit;

namespace RequestFiend.Core.Tests;

public class SecretEncryptorTests : TestsBase {
    private static readonly FieldInfo keyStoreFieldInfo = typeof(SecretEncryptor).GetField("keyStore", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException();

    private static Dictionary<ISecretOwner, byte[]> GetKeyStore(SecretEncryptor subject)
        => keyStoreFieldInfo.GetValue(subject) as Dictionary<ISecretOwner, byte[]> ?? throw new InvalidOperationException();

    [Fact]
    public void TryUnlock_Without_EncryptionData() {
        var owner = Substitute.For<ISecretOwner>();
        owner.EncryptionData.ReturnsNull();

        using var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);

        Assert.True(subject.TryUnlock(owner, "password"));

        var key = Assert.Contains(owner, keyStore);
        Assert.Equal(SHA256.HashSizeInBytes, key.Length);
        Assert.NotEqual(new byte[SecretEncryptor.BlockSizeInBytes], key);

        owner.Received().EncryptionData = Arg.Any<string>();
    }

    [Fact]
    public void TryUnlock_With_EncryptionData() {
        var owner = Substitute.For<ISecretOwner>();
        owner.EncryptionData.Returns("aAWgMAiTzerdlc/Fv63ZVhhLOLakNnhAedqerlPHiSESAEuTlT81WTsLBfzZ");

        using var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);

        Assert.True(subject.TryUnlock(owner, "password"));

        var key = Assert.Contains(owner, keyStore);
        Assert.Equal(SHA256.HashSizeInBytes, key.Length);
        Assert.NotEqual(new byte[SecretEncryptor.BlockSizeInBytes], key);

        owner.DidNotReceive().EncryptionData = Arg.Any<string>();
    }

    [Fact]
    public void TryUnlock_With_EncryptionData_And_Wrong_Password() {
        var owner = Substitute.For<ISecretOwner>();
        owner.EncryptionData.Returns("aAWgMAiTzerdlc/Fv63ZVhhLOLakNnhAedqerlPHiSESAEuTlT81WTsLBfzZ");

        using var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);

        Assert.False(subject.TryUnlock(owner, "wrong"));

        Assert.DoesNotContain(owner, keyStore);
    }

    [Fact]
    public void TryUnlock_When_Unlocked() {
        var owner = Substitute.For<ISecretOwner>();
        owner.EncryptionData.Returns("aAWgMAiTzerdlc/Fv63ZVhhLOLakNnhAedqerlPHiSESAEuTlT81WTsLBfzZ");

        using var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);
        var key = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);
        var originalKey = key.ToArray();
        keyStore[owner] = key;

        Assert.True(subject.TryUnlock(owner, "password"));

        var newKey = Assert.Contains(owner, keyStore);
        Assert.Equal(new byte[SecretEncryptor.BlockSizeInBytes], key);
        Assert.NotEqual(new byte[SecretEncryptor.BlockSizeInBytes], newKey);
        Assert.NotEqual(originalKey, newKey);
    }

    [Fact]
    public void TryUnlock_Throws_When_Disposed() {
        SecretEncryptor subject;

        using (var encryptor = new SecretEncryptor()) {
            subject = encryptor;
        }

        Assert.Throws<ObjectDisposedException>(() => subject.TryUnlock(Substitute.For<ISecretOwner>(), "password"));
    }

    [Fact]
    public void Lock_When_Unlocked() {
        var owner = Substitute.For<ISecretOwner>();
        var otherOwner = Substitute.For<ISecretOwner>();

        using var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);
        var key = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);
        keyStore[owner] = key;
        keyStore[otherOwner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        subject.Lock(owner);

        Assert.DoesNotContain(owner, keyStore);
        Assert.Contains(otherOwner, keyStore);
        Assert.Equal(new byte[SecretEncryptor.BlockSizeInBytes], key);
    }

    [Fact]
    public void Lock_When_Locked() {
        var otherOwner = Substitute.For<ISecretOwner>();

        using var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);
        keyStore[otherOwner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        subject.Lock(Substitute.For<ISecretOwner>());

        Assert.Contains(otherOwner, keyStore);
    }

    [Fact]
    public void Lock_Throws_When_Disposed() {
        SecretEncryptor subject;

        using (var encryptor = new SecretEncryptor()) {
            subject = encryptor;
        }

        Assert.Throws<ObjectDisposedException>(() => subject.Lock(Substitute.For<ISecretOwner>()));
    }

    [Fact]
    public void IsLocked() {
        var owner = Substitute.For<ISecretOwner>();

        using var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);
        keyStore[owner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        Assert.False(subject.IsLocked(owner));
        Assert.True(subject.IsLocked(Substitute.For<ISecretOwner>()));
    }

    [Fact]
    public void IsLocked_Throws_When_Disposed() {
        SecretEncryptor subject;

        using (var encryptor = new SecretEncryptor()) {
            subject = encryptor;
        }

        Assert.Throws<ObjectDisposedException>(() => subject.IsLocked(Substitute.For<ISecretOwner>()));
    }

    [Fact]
    public void TryEncrypt_And_TryDecrypt() {
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();

        using var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);
        keyStore[owner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        Assert.True(subject.TryEncrypt(owner, value, out var encryptResult));
        Assert.NotNull(encryptResult);
        Assert.True(subject.TryDecrypt(owner, encryptResult, out var decryptResult));
        Assert.Equal(value, decryptResult);
    }

    [Fact]
    public void TryEncrypt_Returns_Null_For_Missing_Password() {
        var owner = Substitute.For<ISecretOwner>();

        using var subject = new SecretEncryptor();

        Assert.False(subject.TryEncrypt(owner, "Plain text", out var result));
        Assert.Null(result);
    }

    [Fact]
    public void TryEncrypt_Throws_When_Disposed() {
        SecretEncryptor subject;

        using (var encryptor = new SecretEncryptor()) {
            subject = encryptor;
        }

        Assert.Throws<ObjectDisposedException>(() => subject.TryEncrypt(Substitute.For<ISecretOwner>(), "Plain text", out _));
    }

    [Fact]
    public void TryDecrypt_Returns_Null_For_Missing_Password() {
        var owner = Substitute.For<ISecretOwner>();

        using var subject = new SecretEncryptor();

        Assert.False(subject.TryDecrypt(owner, "Encrypted text", out var result));
        Assert.Null(result);
    }

    [Fact]
    public void TryDecrypt_Throws_When_Disposed() {
        SecretEncryptor subject;

        using (var encryptor = new SecretEncryptor()) {
            subject = encryptor;
        }

        Assert.Throws<ObjectDisposedException>(() => subject.TryDecrypt(Substitute.For<ISecretOwner>(), "Encrypted text", out _));
    }

    [Fact]
    public void Dispose() {
        Dictionary<ISecretOwner, byte[]> keyStore;

        using (var subject = new SecretEncryptor()) {
            keyStore = GetKeyStore(subject);
            keyStore[Substitute.For<ISecretOwner>()] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);
            keyStore[Substitute.For<ISecretOwner>()] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);
        }

        foreach (var key in keyStore.Values) {
            Assert.Equal(new byte[SecretEncryptor.BlockSizeInBytes], key);
        }
    }
}
