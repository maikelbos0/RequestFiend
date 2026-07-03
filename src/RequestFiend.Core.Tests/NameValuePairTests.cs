using Xunit;

namespace RequestFiend.Core.Tests;

public class NameValuePairTests {
    [Fact]
    public void CreateSnapshot() {
        var subject = new NameValuePair() {
            Name = "Name",
            Value = "Value"
        };

        var result = subject.CreateSnapshot();

        Assert.Equal(subject.Name, result.Name);
        Assert.Equal(subject.Value, result.Value);
    }
}
