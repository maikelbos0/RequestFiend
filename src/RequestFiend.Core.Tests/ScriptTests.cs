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

    [Fact]
    public void CreateSnapshot() {
        var subject = new Script() {
            References = { "Foo", "Bar" },
            Code = "Code"
        };

        var result = subject.CreateSnapshot();

        Assert.Equal(subject.References, result.References);
        Assert.Equal(subject.Code, result.Code);
    }
}
