using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class SecretModelTests : TestsBase {
    [Fact]
    public void Name() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Decrypt(owner, ciphertext).Returns(value);

        var secret = new Secret() { Name = "PreviousName", Ciphertext = ciphertext };

        var subject = new SecretModel(owner, secret) {
            Name = { Value = "Name" }
        };

        subject.Name.Set();

        Assert.Equal("Name", secret.Name);
    }

    [Fact]
    public void Value() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Encrypt(owner, value).Returns(ciphertext);

        var secret = new Secret() { Name = "PreviousName" };

        var subject = new SecretModel(owner, secret) {
            Value = { Value = value }
        };

        subject.Value.Set();

        Assert.Equal(ciphertext, secret.Ciphertext);
    }

    [Fact]
    public void Constructor() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Decrypt(owner, ciphertext).Returns(value);

        var secret = new Secret() { Name = "Name", Ciphertext = "Ciphertext" };

        var subject = new SecretModel(owner, secret);

        Assert.Equal(secret.Name, subject.Name.Value);
        Assert.Equal(value, subject.Value.Value);
        Assert.Equal([subject.Name, subject.Value], subject.Validatables);
    }

    [Fact]
    public void Set_When_Password_Can_Be_Provided() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();
        passwordProvider.CanProvide(owner).Returns(true);
        secretEncryptor.Encrypt(owner, value).Returns(ciphertext);

        var secret = new Secret() { Name = "PreviousName" };

        var subject = new SecretModel(owner, secret) {
            Name = { Value = "Name" },
            Value = { Value = value }
        };

        subject.Set();

        Assert.False(subject.IsModified);
        Assert.False(subject.Name.IsModified);
        Assert.False(subject.Value.IsModified);

        Assert.Equal(subject.Name.Value, secret.Name);
        Assert.Equal(ciphertext, secret.Ciphertext);
    }

    [Fact]
    public void Set_When_Password_Cannot_Be_Provided() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();
        passwordProvider.CanProvide(owner).Returns(false);
        secretEncryptor.Encrypt(owner, value).Returns(ciphertext);

        var secret = new Secret() { Name = "PreviousName" };

        var subject = new SecretModel(owner, secret) {
            Name = { Value = "Name" },
            Value = { Value = value }
        };

        subject.Set();

        Assert.True(subject.IsModified);
        Assert.True(subject.Name.IsModified);
        Assert.True(subject.Value.IsModified);
    }
}
