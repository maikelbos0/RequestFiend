using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests {
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void ApplyVariables_Returns_Value_If_Whitespace(string value) {
        var subject = new RequestTemplateCollection() {
            Variables = {
                { "First", "Replacement" },
                { "Second", "Another" }
            }
        };

        var result = subject.ApplyVariables(value);

        Assert.Equal(value, result);
    }

    [Fact]
    public void ApplyVariables_Replaces_Variables_With_Values() {
        var subject = new RequestTemplateCollection() {
            Variables = {
                { "First", "Replacement" },
                { "Second", "Another" }
            }
        };

        var result = subject.ApplyVariables("{{First}} first and {{second}}");

        Assert.Equal("Replacement first and Another", result);
    }
}
