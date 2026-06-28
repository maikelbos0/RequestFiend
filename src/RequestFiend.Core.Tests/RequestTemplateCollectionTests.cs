using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests {
    [Fact]
    public void CreateVariableSnapshot() {
        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Foo", Value = "FooValue" }
            }
        };

        subject.GetSessionVariables().Add("Bar", "BarValue");

        var result = subject.CreateVariableSnapshot(null);

        Assert.Equal(2, result.Variables.Count);
    }

    [Fact]
    public void CreateVariableSnapshot_With_Environment() {
        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Foo", Value = "FooValue" }
            }
        };

        subject.GetSessionVariables().Add("Bar", "BarValue");

        var environment = new Environment() {
            Variables = {
                new() { Name = "Baz", Value = "BazValue" }
            }
        };

        var result = subject.CreateVariableSnapshot(environment);

        Assert.Equal(3, result.Variables.Count);
    }
}
