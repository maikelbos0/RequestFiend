using NSubstitute;
using Xunit;

namespace RequestFiend.Core.Tests;

public class SecretEncryptorTests : TestsBase {
    [Fact]
    public void Encrypt_And_Decrypt() {
        const string value = "Plain text";

        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.Provide(secretOwner).Returns("password");

        var subject = new SecretEncryptor(passwordProvider);

        var encryptResult = subject.Encrypt(secretOwner, value);

        Assert.NotEqual(value, encryptResult);

        var decryptResult = subject.Decrypt(secretOwner, encryptResult);

        Assert.Equal(value, decryptResult);
    }
}
