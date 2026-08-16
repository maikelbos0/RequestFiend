using NSubstitute;
using Xunit;

namespace RequestFiend.Core.Tests;

public class SecretEncryptorTests : TestsBase {
    [Fact]
    public void TryEncrypt_And_TryDecrypt() {
        const string value = "Plain text";

        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.TryProvide(secretOwner, out Arg.Any<string?>()).Returns(callInfo => {
            callInfo[1] = "password";
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
        passwordProvider.TryProvide(secretOwner, out Arg.Any<string?>()).Returns(false);

        var subject = new SecretEncryptor(passwordProvider);

        Assert.False(subject.TryEncrypt(secretOwner, "Plain text", out var result));
        Assert.Null(result);
    }

    [Fact]
    public void Decrypt_Returns_Null_For_Missing_Password() {
        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.TryProvide(secretOwner, out Arg.Any<string?>()).Returns(false);

        var subject = new SecretEncryptor(passwordProvider);

        Assert.False(subject.TryDecrypt(secretOwner, "Encrypted text", out var result));
        Assert.Null(result);
    }
}
