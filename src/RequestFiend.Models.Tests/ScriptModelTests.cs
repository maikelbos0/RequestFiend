using RequestFiend.Core;
using Xunit;
using Xunit.Internal;

namespace RequestFiend.Models.Tests;

public class ScriptModelTests {
    [Fact]
    public void References() {
        var script = new Script();
        var subject = new ScriptModel(script) {
            References = { Value = $" Foo {System.Environment.NewLine}{System.Environment.NewLine} Bar " }
        };

        Assert.Equal(["Foo", "Bar"], script.References);
    }

    [Fact]
    public void Code() {
        var script = new Script();
        var subject = new ScriptModel(script) {
            Code = { Value = "Code" }
        };

        Assert.Equal("Code", script.Code);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true, "Foo", "Bar")]
    public void Constructor(bool expectedShowReferences, params string[] references) {
        var script = new Script() {
            References = [.. references],
            Code = "Script"
        };

        var subject = new ScriptModel(script);

        Assert.Equal(expectedShowReferences, subject.ShowReferences);
        Assert.Equal(string.Join(System.Environment.NewLine, script.References), subject.References.Value);
        Assert.Equal(script.Code, subject.Code.Value);

        Assert.Equal([subject.References, subject.Code], subject.Validatables);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void ToggleShowReferences(bool initialValue, bool expectedValue) {
        var subject = new ScriptModel(new()) {
            ShowReferences = initialValue
        };

        subject.ToggleShowReferences();

        Assert.Equal(expectedValue, subject.ShowReferences);
    }
}
