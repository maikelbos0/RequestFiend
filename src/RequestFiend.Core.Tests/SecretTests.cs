using NSubstitute;
using Xunit;

namespace RequestFiend.Core.Tests;

public class SecretTests : TestsBase {
    [Fact]
    public void GetPlaintextValue() {
        const string encryptedValue = "EncryptedValue";
        const string plaintextValue = "PlaintextValue";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.TryDecrypt(owner, encryptedValue, out Arg.Any<string?>()).Returns(callInfo => {
            callInfo[2] = plaintextValue;
            return true;
        });

        var subject = new Secret() {
            Name = "Name",
            EncryptedValue = encryptedValue
        };

        var result = subject.GetPlaintextValue(owner);

        Assert.Equal(plaintextValue, result);
    }
    [Fact]
    public void GetPlaintextValue_Returns_Masked_String_For_False_Decrypt_Result() {
        const string encryptedValue = "EncryptedValue";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.TryDecrypt(owner, encryptedValue, out Arg.Any<string?>()).Returns(false);

        var subject = new Secret() {
            Name = "Name",
            EncryptedValue = encryptedValue
        };

        var result = subject.GetPlaintextValue(owner);

        Assert.Equal("********", result);
    }

    [Fact]
    public void GetPlaintextValue_Returns_Empty_String_For_Null_EncryptedValue() {
        var owner = Substitute.For<ISecretOwner>();

        var subject = new Secret() {
            Name = "Name"
        };

        var result = subject.GetPlaintextValue(owner);

        Assert.Empty(result);

        secretEncryptor.DidNotReceive().TryDecrypt(owner, Arg.Any<string>(), out Arg.Any<string?>());
    }

    [Fact]
    public void SetPlaintextValue() {
        const string encryptedValue = "EncryptedValue";
        const string plaintextValue = "PlaintextValue";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.TryEncrypt(owner, plaintextValue, out Arg.Any<string?>()).Returns(callInfo => {
            callInfo[2] = encryptedValue;
            return true;
        });

        var subject = new Secret() {
            Name = "Name"
        };

        subject.SetPlaintextValue(owner, plaintextValue);

        Assert.Equal(encryptedValue, subject.EncryptedValue);
    }

    [Fact]
    public void SetPlaintextValue_Keeps_Previous_EncryptedValue_For_False_Encrypt_Result() {
        const string plaintextValue = "PlaintextValue";

        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.TryEncrypt(owner, plaintextValue, out Arg.Any<string?>()).Returns(false);

        var subject = new Secret() {
            Name = "Name",
            EncryptedValue = "PreviousEncryptedValue"
        };

        subject.SetPlaintextValue(owner, plaintextValue);

        Assert.Equal("PreviousEncryptedValue", subject.EncryptedValue);
    }

    [Fact]
    public void SetPlaintextValue_Sets_EncryptedValue_To_Null_For_Null_PlaintextValue() {
        var owner = Substitute.For<ISecretOwner>();

        var subject = new Secret() {
            Name = "Name",
            EncryptedValue = "PreviousEncryptedValue"
        };

        subject.SetPlaintextValue(owner, null);

        Assert.Null(subject.EncryptedValue);

        secretEncryptor.DidNotReceive().TryEncrypt(owner, Arg.Any<string>(), out Arg.Any<string?>());
    }
}
