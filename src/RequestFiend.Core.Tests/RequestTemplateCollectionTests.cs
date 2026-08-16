using NSubstitute;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests : TestsBase {
    [Fact]
    public void CreateVariableSnapshot() {
        const string ciphertext = "Ciphertext";

        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Foo", Value = "FooValue" }
            },
            Secrets = {
                new() { Name = "Baz", Ciphertext = "Ciphertext" }
            }
        };

        secretEncryptor.Decrypt(subject, ciphertext).Returns("BazValue");
        subject.GetSessionVariables().Add("Bar", "BarValue");

        var result = subject.CreateVariableSnapshot(null);

        Assert.Equal(3, result.Variables.Count);

        secretEncryptor.Received(1).Decrypt(subject, ciphertext);
    }

    [Fact]
    public void CreateVariableSnapshot_With_Environment() {
        const string ciphertext = "Ciphertext";

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
                new() { Name = "Baz", Ciphertext = "Ciphertext" }
            }
        };

        secretEncryptor.Decrypt(environment, ciphertext).Returns("BazValue");

        var result = subject.CreateVariableSnapshot(environment);

        Assert.Equal(3, result.Variables.Count);

        secretEncryptor.Received(1).Decrypt(environment, ciphertext);
    }
}
