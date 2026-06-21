using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests {
    [Fact]
    public void GetVariableSnapshot() {
        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "Foo", Value = "FooValue" }
            }
        };

        subject.GetSessionVariables().Add("Bar", "BarValue");

        var result = subject.GetVariableSnapshot(null);

        Assert.Equal(2, result.Variables.Count);
    }

    [Fact]
    public void GetVariableSnapshot_With_Environment() {
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

        var result = subject.GetVariableSnapshot(environment);

        Assert.Equal(3, result.Variables.Count);
    }
}
