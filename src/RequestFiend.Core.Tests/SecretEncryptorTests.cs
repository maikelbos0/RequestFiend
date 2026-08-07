using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class SecretEncryptorTests {
    [Fact]
    public async Task Encrypt_And_Decrypt() {
        const string value = "Plain text";

        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.Provide(secretOwner).Returns("password");

        var subject = new SecretEncryptor(passwordProvider);

        var encryptedValue = await subject.Encrypt(secretOwner, value);

        Assert.NotNull(encryptedValue);

        var decryptedValue = await subject.Decrypt(secretOwner, encryptedValue);

        Assert.Equal(value, decryptedValue);
    }

    [Fact]
    public async Task Encrypt_Returns_Null_For_Missing_Password() {
        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.Provide(secretOwner).ReturnsNull();

        var subject = new SecretEncryptor(passwordProvider);

        var result = await subject.Encrypt(secretOwner, "Plain text");

        Assert.Null(result);
    }

    [Fact]
    public async Task Decrypt_Returns_Null_For_Missing_Password() {
        var secretOwner = Substitute.For<ISecretOwner>();

        var passwordProvider = Substitute.For<IPasswordProvider>();
        passwordProvider.Provide(secretOwner).ReturnsNull();

        var subject = new SecretEncryptor(passwordProvider);

        var result = await subject.Decrypt(secretOwner, "Encrypted text");

        Assert.Null(result);
    }
}
