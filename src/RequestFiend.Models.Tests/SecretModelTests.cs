using NSubstitute;
using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class SecretModelTests : TestsBase {
    [Fact]
    public void Name() {
        var secret = new Secret() { Name = "PreviousName" };

        var subject = new SecretModel(Substitute.For<ISecretOwner>(), secret) {
            Name = { Value = "Name" }
        };

        subject.Name.Set();

        Assert.Equal("Name", secret.Name);
    }

    [Fact]
    public void Value() {
        const string encryptedValue = "EncryptedValue";
        const string plaintextValue = "PlaintextValue";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.TryEncrypt(owner, plaintextValue, out Arg.Any<string?>()).Returns(callInfo => {
            callInfo[2] = encryptedValue;
            return true;
        });

        var secret = new Secret() { Name = "PreviousName" };

        var subject = new SecretModel(owner, secret) {
            Value = { Value = plaintextValue }
        };

        subject.Value.Set();

        Assert.Equal(encryptedValue, secret.EncryptedValue);
    }

    [Fact]
    public void Constructor() {
        const string encryptedValue = "EncryptedValue";
        const string plaintextValue = "PlaintextValue";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.TryDecrypt(owner, encryptedValue, out Arg.Any<string?>()).Returns(callInfo => {
            callInfo[2] = plaintextValue;
            return true;
        });

        var secret = new Secret() { Name = "Name", EncryptedValue = encryptedValue };

        var subject = new SecretModel(owner, secret);

        Assert.Equal(secret.Name, subject.Name.Value);
        Assert.Equal(plaintextValue, subject.Value.Value);
        Assert.Equal([subject.Name, subject.Value], subject.Validatables);
    }

    [Fact]
    public void Set_When_Password_Can_Be_Provided() {
        const string encryptedValue = "EncryptedValue";
        const string plaintextValue = "PlaintextValue";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.IsLocked(owner).Returns(false);
        secretEncryptor.TryEncrypt(owner, plaintextValue, out Arg.Any<string?>()).Returns(callInfo => {
            callInfo[2] = encryptedValue;
            return true;
        });

        var secret = new Secret() { Name = "PreviousName" };

        var subject = new SecretModel(owner, secret) {
            Name = { Value = "Name" },
            Value = { Value = plaintextValue }
        };

        subject.Set();

        Assert.False(subject.IsModified);
        Assert.False(subject.Name.IsModified);
        Assert.False(subject.Value.IsModified);

        Assert.Equal(subject.Name.Value, secret.Name);
        Assert.Equal(encryptedValue, secret.EncryptedValue);
    }

    [Fact]
    public void Set_When_Password_Cannot_Be_Provided() {
        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.IsLocked(owner).Returns(true);

        var secret = new Secret() { Name = "PreviousName" };

        var subject = new SecretModel(owner, secret) {
            Name = { Value = "Name" },
            Value = { Value = "PlaintextValue" }
        };

        subject.Set();

        Assert.True(subject.IsModified);
        Assert.True(subject.Name.IsModified);
        Assert.True(subject.Value.IsModified);
    }
}
