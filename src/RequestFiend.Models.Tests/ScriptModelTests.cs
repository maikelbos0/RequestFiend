using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class ScriptModelTests {
    [Fact]
    public void Constructor() {
        var script = new Script() {
            Code = "Script"
        };

        var subject = new ScriptModel(script);

        Assert.Equal(script.Code, subject.Code.Value);

        Assert.Equal([subject.Code], subject.Validatables);
    }

    [Fact]
    public void Update() {
        const string code = "Script";

        var script = new Script();

        var subject = new ScriptModel(script);

        subject.Code.Value = code;
        
        subject.Update(script);

        Assert.Equal(code, script.Code);
    }

    [Fact]
    public void Reset() {
        var script = new Script() {
            Code = "Script"
        };

        var subject = new ScriptModel(script);

        subject.Reset();

        Assert.Equal(script.Code, subject.Code.Value);
        Assert.False(subject.Code.IsModified);
    }
}
