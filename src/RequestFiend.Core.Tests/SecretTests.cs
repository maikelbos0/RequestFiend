using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace RequestFiend.Core.Tests;

public class SecretTests : TestsBase {
    [Fact]
    public void GetValue() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Decrypt(owner, ciphertext).Returns(value);

        var subject = new Secret() {
            Name = "Name",
            Ciphertext = ciphertext
        };

        var result = subject.GetValue(owner);

        Assert.Equal(value, result);
    }
    [Fact]
    public void GetValue_Returns_Empty_String_For_Null_Decrypt_Result() {
        const string ciphertext = "Ciphertext";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Decrypt(owner, ciphertext).ReturnsNull();

        var subject = new Secret() {
            Name = "Name",
            Ciphertext = ciphertext
        };

        var result = subject.GetValue(owner);

        Assert.Empty(result);
    }

    [Fact]
    public void GetValue_Returns_Empty_String_For_Null_Ciphertext() {
        var owner = Substitute.For<ISecretOwner>();

        var subject = new Secret() {
            Name = "Name"
        };

        var result = subject.GetValue(owner);

        Assert.Empty(result);

        secretEncryptor.DidNotReceive().Decrypt(owner, Arg.Any<string>());
    }

    [Fact]
    public void SetValue() {
        const string ciphertext = "Ciphertext";
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Encrypt(owner, value).Returns(ciphertext);

        var subject = new Secret() {
            Name = "Name"
        };

        subject.SetValue(owner, value);

        Assert.Equal(ciphertext, subject.Ciphertext);
    }

    [Fact]
    public void SetValue_Keeps_Previous_Ciphertext_For_Null_Encrypt_Result() {
        const string value = "Plain text";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.Encrypt(owner, value).ReturnsNull();

        var subject = new Secret() {
            Name = "Name",
            Ciphertext = "PreviousCiphertext"
        };

        subject.SetValue(owner, value);

        Assert.Equal("PreviousCiphertext", subject.Ciphertext);
    }

    [Fact]
    public void SetValue_Sets_Ciphertext_To_Null_For_Null_Value() {
        var owner = Substitute.For<ISecretOwner>();

        var subject = new Secret() {
            Name = "Name",
            Ciphertext = "PreviousCiphertext"
        };

        subject.SetValue(owner, null);

        Assert.Null(subject.Ciphertext);

        secretEncryptor.DidNotReceive().Encrypt(owner, Arg.Any<string>());
    }
}
