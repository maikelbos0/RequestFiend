using NSubstitute;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests : TestsBase {
    [Fact]
    public void CreateVariableSnapshot() {
        const string encryptedValue = "EncryptedValue";

        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Foo", Value = "FooValue" }
            },
            Secrets = {
                new() { Name = "Baz", EncryptedValue = encryptedValue }
            }
        };

        subject.GetSessionVariables().Add("Bar", "BarValue");

        var result = subject.CreateVariableSnapshot(null);

        Assert.Equal(3, result.Variables.Count);

        secretEncryptor.Received(1).TryDecrypt(subject, encryptedValue, out Arg.Any<string?>());
    }

    [Fact]
    public void CreateVariableSnapshot_With_Environment() {
        const string encryptedValue = "EncryptedValue";

        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Foo", Value = "FooValue" }
            }
        };

        var environment = new Environment() {
            Variables = {
                new() { Name = "Bar", Value = "BazValue" }
            },
            Secrets = {
                new() { Name = "Baz", EncryptedValue = encryptedValue }
            }
        };

        var result = subject.CreateVariableSnapshot(environment);

        Assert.Equal(3, result.Variables.Count);

        secretEncryptor.Received(1).TryDecrypt(environment, encryptedValue, out Arg.Any<string?>());
    }
}
