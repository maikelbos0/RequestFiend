using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace RequestFiend.Models.Tests;

public class VariableModelTests {
    [Fact]
    public void CreateRange() {
        var variables = ImmutableDictionary.CreateRange([
            KeyValuePair.Create("{{Foo}}", "FooValue"),
            KeyValuePair.Create("{{Bar}}", "BarValue")
        ]);

        var subject = VariableModel.CreateRange(variables);

        Assert.Equal(variables.Count, subject.Length);

        foreach (var variable in variables) {
            Assert.Contains(subject, variableModel => variableModel.Key == variable.Key && variableModel.Value == variable.Value);
        }
    }
}
