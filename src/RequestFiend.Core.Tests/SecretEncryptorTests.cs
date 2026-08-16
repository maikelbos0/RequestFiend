using NSubstitute;
using Xunit;

namespace RequestFiend.Core.Tests;

public class SecretEncryptorTests : TestsBase {
    [Fact]
    public void Encrypt_And_Decrypt() {
        const string value = "Plain text";

        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.TryProvide(secretOwner, out Arg.Any<string?>()).Returns(callInfo => {
            callInfo[1] = "password";
            return true;
        });

        var subject = new SecretEncryptor(passwordProvider);

        var encryptResult = subject.Encrypt(secretOwner, value);

        Assert.NotNull(encryptResult);
        Assert.NotEqual(value, encryptResult);

        var decryptResult = subject.Decrypt(secretOwner, encryptResult);

        Assert.Equal(value, decryptResult);
    }

    [Fact]
    public void Encrypt_Returns_Null_For_Missing_Password() {
        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.TryProvide(secretOwner, out Arg.Any<string?>()).Returns(false);

        var subject = new SecretEncryptor(passwordProvider);

        var result = subject.Encrypt(secretOwner, "Plain text");

        Assert.Null(result);
    }

    [Fact]
    public void Decrypt_Returns_Null_For_Missing_Password() {
        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.TryProvide(secretOwner, out Arg.Any<string?>()).Returns(false);

        var subject = new SecretEncryptor(passwordProvider);

        var result = subject.Decrypt(secretOwner, "Encrypted text");

        Assert.Null(result);
    }
}
