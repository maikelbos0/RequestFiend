using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateCollectionTests {
    [Fact]
    public void ApplyVariables_Replaces_Variables_With_Values() {
        var subject = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "First", Value = "Replacement" },
                new() { Name = "Second", Value = "Another" }
            }
        };

        var result = subject.ApplyVariables("{{First}} first and {{second}}");

        Assert.Equal("Replacement first and Another", result);
    }
}
