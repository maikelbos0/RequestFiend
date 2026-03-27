using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests {
    [Theory]
    [InlineData("Foo")]
    [InlineData("123")]
    [InlineData("Да")]
    [InlineData("٢٣٤٥٦٧٨٩")]
    [InlineData("１２３")]
    [InlineData("Foo_132")]
    public void ApplyVariables_Replaces_Variable_With_Value(string name) {
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

        var result = subject.ApplyVariables(input);

        Assert.Equal(input, result);
    }
}
