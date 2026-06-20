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
}
