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

        var result = subject.GetVariableSnapshot();

        Assert.Equal(2, result.Variables.Count);
    }

    [Theory]
    [InlineData("Foo")]
    [InlineData("123")]
    [InlineData("Да")]
    [InlineData("٢٣٤٥٦٧٨٩")]
    [InlineData("１２３")]
    [InlineData("Foo_132")]
    public void ApplyVariables_Replaces_Variable_With_Session_Value(string name) {
        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = name, Value = "Duplicate" }
            }
        };

        subject.GetSessionVariables().Add(name, "Replacement");

        var result = subject.ApplyVariables($"We change {{{{{name}}}}} to Replacement");

        Assert.Equal("We change Replacement to Replacement", result);
    }

    [Theory]
    [InlineData("Foo")]
    [InlineData("123")]
    [InlineData("Да")]
    [InlineData("٢٣٤٥٦٧٨٩")]
    [InlineData("１２３")]
    [InlineData("Foo_132")]
    public void ApplyVariables_Replaces_Variable_With_Persisted_Value(string name) {
        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = name, Value = "Replacement" }
            }
        };

        var result = subject.ApplyVariables($"We change {{{{{name}}}}} to Replacement");

        Assert.Equal("We change Replacement to Replacement", result);
    }

    [Theory]
    [InlineData("Foo+132")]
    [InlineData("Foo 132")]
    [InlineData("Foo/132")]
    public void ApplyVariables_Ignores_Invalid_Variables(string name) {
        var input = $"We can leave {{{{{name}}}}} as-is";

        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = name, Value = "Replacement" }
            }
        };

        subject.GetSessionVariables().Add(name, "Replacement");

        var result = subject.ApplyVariables(input);

        Assert.Equal(input, result);
    }
}
