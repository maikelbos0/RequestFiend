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
    private static readonly FieldInfo keysFieldInfo = typeof(SecretEncryptor).GetField("keys", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException();

    private static Dictionary<ISecretOwner, byte[]> GetKeys(SecretEncryptor subject)
        => keysFieldInfo.GetValue(subject) as Dictionary<ISecretOwner, byte[]> ?? throw new InvalidOperationException();

    [Fact]
    public void Unlock_Without_Salt() {
        var secretOwner = Substitute.For<ISecretOwner>();
        secretOwner.Salt.ReturnsNull();

        var subject = new SecretEncryptor(Substitute.For<IPasswordProvider>());
        var keys = GetKeys(subject);

        subject.Unlock(secretOwner, "password");

        var key = Assert.Contains(secretOwner, keys);
        Assert.Equal(SHA256.HashSizeInBytes, key.Length);
        Assert.NotEqual(new byte[SecretEncryptor.BlockSizeInBytes], key);

        secretOwner.Received().Salt = Arg.Is<byte[]>(salt => salt.Length == SecretEncryptor.BlockSizeInBytes);
    }

    [Fact]
    public void Unlock_With_Salt() {
        var salt = new byte[SecretEncryptor.BlockSizeInBytes];
        var pwner = Substitute.For<ISecretOwner>();
        pwner.Salt.Returns(salt);

        var subject = new SecretEncryptor(Substitute.For<IPasswordProvider>());
        var keys = GetKeys(subject);

        subject.Unlock(pwner, "password");

        var key = Assert.Contains(pwner, keys);
        Assert.Equal(SHA256.HashSizeInBytes, key.Length);
        Assert.NotEqual(new byte[SecretEncryptor.BlockSizeInBytes], key);

        pwner.DidNotReceive().Salt = Arg.Any<byte[]>();
    }
    
    [Fact]
    public void Lock_When_Unlocked() {
        var owner = Substitute.For<ISecretOwner>();
        var otherOwner = Substitute.For<ISecretOwner>();

        var subject = new SecretEncryptor(Substitute.For<IPasswordProvider>());
        var keys = GetKeys(subject);
        var key = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);
        keys[owner] = key;
        keys[otherOwner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        subject.Lock(owner);

        Assert.DoesNotContain(owner, keys);
        Assert.Contains(otherOwner, keys);
        Assert.Equal(new byte[SecretEncryptor.BlockSizeInBytes], key);
    }
    
    [Fact]
    public void Lock_When_Locked() {
        var otherOwner = Substitute.For<ISecretOwner>();

        var subject = new SecretEncryptor(Substitute.For<IPasswordProvider>());
        var keys = GetKeys(subject);
        keys[otherOwner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        subject.Lock(Substitute.For<ISecretOwner>());

        Assert.Contains(otherOwner, keys);
    }

    [Fact]
    public void IsLocked() {
        var owner = Substitute.For<ISecretOwner>();

        var subject = new SecretEncryptor(Substitute.For<IPasswordProvider>());
        var keys = GetKeys(subject);
        keys[owner] = RandomNumberGenerator.GetBytes(SecretEncryptor.BlockSizeInBytes);

        Assert.False(subject.IsLocked(owner));
        Assert.True(subject.IsLocked(Substitute.For<ISecretOwner>()));
    }

    [Fact]
    public void TryEncrypt_And_TryDecrypt() {
        const string value = "Plain text";

        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.TryProvide(secretOwner, out Arg.Any<ReadOnlyMemory<byte>>()).Returns(callInfo => {
            callInfo[1] = new ReadOnlyMemory<byte>("password"u8.ToArray());
            return true;
        });

        var subject = new SecretEncryptor(passwordProvider);

        Assert.True(subject.TryEncrypt(secretOwner, value, out var encryptResult));
        Assert.NotNull(encryptResult);
        Assert.True(subject.TryDecrypt(secretOwner, encryptResult, out var decryptResult));
        Assert.Equal(value, decryptResult);
    }

    [Fact]
    public void Encrypt_Returns_Null_For_Missing_Password() {
        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.TryProvide(secretOwner, out Arg.Any<ReadOnlyMemory<byte>>()).Returns(false);

        var subject = new SecretEncryptor(passwordProvider);

        Assert.False(subject.TryEncrypt(secretOwner, "Plain text", out var result));
        Assert.Null(result);
    }

    [Fact]
    public void Decrypt_Returns_Null_For_Missing_Password() {
        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.TryProvide(secretOwner, out Arg.Any<ReadOnlyMemory<byte>>()).Returns(false);

        var subject = new SecretEncryptor(passwordProvider);

        Assert.False(subject.TryDecrypt(secretOwner, "Encrypted text", out var result));
        Assert.Null(result);
    }
}
