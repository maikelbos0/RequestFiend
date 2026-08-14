using NSubstitute;
using Xunit;

namespace RequestFiend.Core.Tests;

public class SecretTests : TestsBase {
    [Fact]
    public void GetValue() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Decrypt(owner, ciphertext).Returns(value);

        AppHost.Services.GetService(typeof(ISecretEncryptor)).Returns(secretEncryptor);

        var subject = new Secret() {
            Name = "Name",
            Ciphertext = ciphertext
        };

        var result = subject.GetValue(owner);

        Assert.Equal(value, result);
    }
    
    [Fact]
    public void GetValue_Returns_Null_For_Null_Ciphertext() {
        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();

        AppHost.Services.GetService(typeof(ISecretEncryptor)).Returns(secretEncryptor);

        var subject = new Secret() {
            Name = "Name"
        };

        var result = subject.GetValue(owner);

        Assert.Null(result);

        secretEncryptor.DidNotReceive().Decrypt(owner, Arg.Any<string>());
    }

    [Fact]
    public void SetValue() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Encrypt(owner, value).Returns(ciphertext);

        AppHost.Services.GetService(typeof(ISecretEncryptor)).Returns(secretEncryptor);

        var subject = new Secret() {
            Name = "Name"
        };

        subject.SetValue(owner, value);

        Assert.Equal(ciphertext, subject.Ciphertext);
    }

    [Fact]
    public void SetValue_Sets_Ciphertext_To_Null_For_Null_Value() {
        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();

        AppHost.Services.GetService(typeof(ISecretEncryptor)).Returns(secretEncryptor);

        var subject = new Secret() {
            Name = "Name",
            Ciphertext = "PreviousCiphertext"
        };

        subject.SetValue(owner, null);

        Assert.Null(subject.Ciphertext);

        secretEncryptor.DidNotReceive().Encrypt(owner, Arg.Any<string>());
    }
}
