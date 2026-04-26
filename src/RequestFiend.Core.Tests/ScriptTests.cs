using Xunit;

namespace RequestFiend.Core.Tests;

public class ScriptTests {
    [Fact]
    public void Clone() {
        var subject = new Script() {
            Code = "Code"
        };

        var result = subject.Clone();

        Assert.NotSame(subject, result);
        Assert.Equal(subject.Code, result.Code);
    }
}
