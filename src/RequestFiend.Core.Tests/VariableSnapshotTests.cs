using Xunit;

namespace RequestFiend.Core.Tests;

public class VariableSnapshotTests {
    [Fact]
    public void Constructor() {
        var subject = new VariableSnapshot([
            [
                new() { Name = "Foo", Value = "FooValue" },
                new() { Name = "Bar", Value = "BarValue" },
                new() { Name = "Foo", Value = "Duplicate" },
                new() { Name = "123", Value = "123Value" },
                new() { Name = "Да", Value = "ДаValue" },
                new() { Name = "٢٣٤٥٦٧٨٩", Value = "٢٣٤٥٦٧٨٩Value" },
                new() { Name = "Foo+123", Value = "Invalid" },
                new() { Name = "Foo 123", Value = "Invalid" }
            ],
            [
                new() { Name = "Bar", Value = "Duplicate" },
                new() { Name = "１２３", Value = "１２３Value" },
                new() { Name = "Qux_123", Value = "Qux_123Value" },
                new() { Name = "Foo/123", Value = "Invalid" },
                new() { Name = "{Foo}", Value = "Invalid" }
            ]
        ]);

        Assert.Equal(7, subject.Variables.Length);
        Assert.Equal(("{{Foo}}", "FooValue"), subject.Variables[0]);
        Assert.Equal(("{{Bar}}", "BarValue"), subject.Variables[1]);
        Assert.Equal(("{{123}}", "123Value"), subject.Variables[2]);
        Assert.Equal(("{{Да}}", "ДаValue"), subject.Variables[3]);
        Assert.Equal(("{{٢٣٤٥٦٧٨٩}}", "٢٣٤٥٦٧٨٩Value"), subject.Variables[4]);
        Assert.Equal(("{{１２３}}", "１２３Value"), subject.Variables[5]);
        Assert.Equal(("{{Qux_123}}", "Qux_123Value"), subject.Variables[6]);
    }

    [Theory]
    [InlineData("A {{Foo}} here", "A FooValue here")]
    [InlineData("A {{foo}} here", "A FooValue here")]
    [InlineData("A {{FOO}} here", "A FooValue here")]
    [InlineData("A {{123}} there", "A 123Value there")]
    [InlineData("A {{Да}} there", "A ДаValue there")]
    [InlineData("A {{ДА}} there", "A ДаValue there")]
    [InlineData("A {{٢٣٤٥٦٧٨٩}} there", "A ٢٣٤٥٦٧٨٩Value there")]
    [InlineData("A {{１２３}} there", "A １２３Value there")]
    [InlineData("A {{Qux_123}} there", "A Qux_123Value there")]
    [InlineData("A {{{Foo}}, {{٢٣٤٥٦٧٨٩}} or {{１２３}}} everywhere", "A {FooValue, ٢٣٤٥٦٧٨٩Value or １２３Value} everywhere")]
    public void Apply(string value, string expectedResult) {
        var subject = new VariableSnapshot([
            [
                new() { Name = "Foo", Value = "FooValue" },
                new() { Name = "Bar", Value = "BarValue" },
                new() { Name = "Foo", Value = "Duplicate" },
                new() { Name = "123", Value = "123Value" },
                new() { Name = "Да", Value = "ДаValue" },
                new() { Name = "٢٣٤٥٦٧٨٩", Value = "٢٣٤٥٦٧٨٩Value" },
                new() { Name = "Foo+123", Value = "Invalid" },
                new() { Name = "Foo 123", Value = "Invalid" }
            ],
            [
                new() { Name = "Bar", Value = "Duplicate" },
                new() { Name = "１２３", Value = "１２３Value" },
                new() { Name = "Qux_123", Value = "Qux_123Value" },
                new() { Name = "Foo/123", Value = "Invalid" },
                new() { Name = "{Foo}", Value = "Invalid" }
            ]
        ]);

        Assert.Equal(expectedResult, subject.Apply(value));
    }
}
