using NSubstitute;
using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class SecretModelTests : TestsBase {
    [Fact]
    public void Name() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Decrypt(owner, ciphertext).Returns(value);

        AppHost.Services.GetService(typeof(ISecretEncryptor)).Returns(secretEncryptor);

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

        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Encrypt(owner, value).Returns(ciphertext);

        AppHost.Services.GetService(typeof(ISecretEncryptor)).Returns(secretEncryptor);

        var secret = new Secret() { Name = "PreviousName" };

        var subject = new SecretModel(owner, secret) {
            Value = { Value = value }
        };

        subject.Value.Set();

        Assert.Equal(ciphertext, secret.Ciphertext);
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("Name", false)]
    public void Constructor(string name, bool expectedHasError) {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Decrypt(owner, ciphertext).Returns(value);

        AppHost.Services.GetService(typeof(ISecretEncryptor)).Returns(secretEncryptor);

        var secret = new Secret() { Name = name, Ciphertext = "Ciphertext" };

        var subject = new SecretModel(owner, secret);

        Assert.Equal(secret.Name, subject.Name.Value);
        Assert.Equal(expectedHasError, subject.Name.HasError);
        Assert.Equal(value, subject.Value.Value);
        Assert.Equal([subject.Name, subject.Value], subject.Validatables);
    }
}
