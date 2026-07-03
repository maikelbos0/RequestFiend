using Xunit;

namespace RequestFiend.Core.Tests;

public class ScriptTests {
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
