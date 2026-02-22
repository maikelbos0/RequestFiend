using Xunit;

namespace RequestFiend.Core.Tests;

public class NameValuePairTests {
    [Fact]
    public void Clone() {
        var subject = new NameValuePair() {
            Name = "Name",
            Value = "Value"
        };

        var result = subject.Clone(); 

        Assert.Equal(subject.Name, result.Name);
        Assert.Equal(subject.Value, result.Value);
    }
}
