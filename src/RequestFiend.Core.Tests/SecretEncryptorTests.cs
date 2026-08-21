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
    public void Unlock_Without_Salt() {
        var owner = Substitute.For<ISecretOwner>();
        owner.Salt.ReturnsNull();

        var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);

        subject.Unlock(owner, "password");

        var key = Assert.Contains(owner, keyStore);
        Assert.Equal(SHA256.HashSizeInBytes, key.Length);
        Assert.NotEqual(new byte[SecretEncryptor.BlockSizeInBytes], key);

        owner.Received().Salt = Arg.Is<byte[]>(salt => salt.Length == SecretEncryptor.BlockSizeInBytes);
    }

    [Fact]
    public void Unlock_With_Salt() {
        var salt = new byte[SecretEncryptor.BlockSizeInBytes];
        var owner = Substitute.For<ISecretOwner>();
        owner.Salt.Returns(salt);

        var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);

        subject.Unlock(owner, "password");

        var key = Assert.Contains(owner, keyStore);
        Assert.Equal(SHA256.HashSizeInBytes, key.Length);
        Assert.NotEqual(new byte[SecretEncryptor.BlockSizeInBytes], key);

        owner.DidNotReceive().Salt = Arg.Any<byte[]>();
    }

    [Fact]
    public void Unlock_When_Unlocked() {
        var owner = Substitute.For<ISecretOwner>();
        owner.Salt.Returns(new byte[SecretEncryptor.BlockSizeInBytes]);

        var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);
        var key = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);
        var originalKey = key.ToArray();
        keyStore[owner] = key;

        subject.Unlock(owner, "password");

        var newKey = Assert.Contains(owner, keyStore);
        Assert.Equal(new byte[SecretEncryptor.BlockSizeInBytes], key);
        Assert.NotEqual(new byte[SecretEncryptor.BlockSizeInBytes], newKey);
        Assert.NotEqual(originalKey, newKey);
    }
    
    [Fact]
    public void Lock_When_Unlocked() {
        var owner = Substitute.For<ISecretOwner>();
        var otherOwner = Substitute.For<ISecretOwner>();

        var subject = new SecretEncryptor();
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

        var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);
        keyStore[otherOwner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        subject.Lock(Substitute.For<ISecretOwner>());

        Assert.Contains(otherOwner, keyStore);
    }

    [Fact]
    public void IsLocked() {
        var owner = Substitute.For<ISecretOwner>();

        var subject = new SecretEncryptor();
        var keyStore = GetKeyStore(subject);
        keyStore[owner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        Assert.False(subject.IsLocked(owner));
        Assert.True(subject.IsLocked(Substitute.For<ISecretOwner>()));
    }

    [Fact]
    public void TryEncrypt_And_TryDecrypt() {
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();

        var subject = new SecretEncryptor();
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

        var subject = new SecretEncryptor();

        Assert.False(subject.TryEncrypt(owner, "Plain text", out var result));
        Assert.Null(result);
    }

    [Fact]
    public void TryDecrypt_Returns_Null_For_Missing_Password() {
        var owner = Substitute.For<ISecretOwner>();

        var subject = new SecretEncryptor();

        Assert.False(subject.TryDecrypt(owner, "Encrypted text", out var result));
        Assert.Null(result);
    }
}
