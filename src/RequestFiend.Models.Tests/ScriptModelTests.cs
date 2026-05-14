using RequestFiend.Core;
using System;
using Xunit;
using Xunit.Internal;

namespace RequestFiend.Models.Tests;

public class ScriptModelTests {
    [Fact]
    public void Constructor() {
        var script = new Script() {
            References = ["Foo", "Bar"],
            Code = "Script"
        };

        var subject = new ScriptModel(script);

        Assert.Equal(string.Join(Environment.NewLine, script.References), subject.References.Value);
        Assert.Equal(script.Code, subject.Code.Value);

        Assert.Equal([subject.References, subject.Code], subject.Validatables);
    }

    [Fact]
    public void Update() {
        const string code = "Script";

        var script = new Script();

        var subject = new ScriptModel(script);

        subject.References.Value = " \r\n Foo \r\n\r Bar \r\n \r\n";
        subject.Code.Value = code;
        
        subject.Update(script);

        Assert.Equal(["Foo", "Bar"], script.References);
        Assert.Equal(code, script.Code);
    }

    [Fact]
    public void Reset() {
        var script = new Script() {
            References = ["Foo", "Bar"],
            Code = "PreviousScript"
        };

        var subject = new ScriptModel(script);

        script.References.Add("Baz");
        script.Code = "NewScript";

        subject.Reset();

        Assert.Equal(string.Join(Environment.NewLine, script.References), subject.References.Value);
        Assert.False(subject.References.IsModified);
        Assert.Equal(script.Code, subject.Code.Value);
        Assert.False(subject.Code.IsModified);
    }
}
