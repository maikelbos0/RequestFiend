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

        Assert.Equal(7, subject.Variables.Count);
        Assert.Equal("FooValue", Assert.Contains("{{Foo}}", subject.Variables));
        Assert.Equal("BarValue", Assert.Contains("{{Bar}}", subject.Variables));
        Assert.Equal("123Value", Assert.Contains("{{123}}", subject.Variables));
        Assert.Equal("ДаValue", Assert.Contains("{{Да}}", subject.Variables));
        Assert.Equal("٢٣٤٥٦٧٨٩Value", Assert.Contains("{{٢٣٤٥٦٧٨٩}}", subject.Variables));
        Assert.Equal("１２３Value", Assert.Contains("{{１２３}}", subject.Variables));
        Assert.Equal("Qux_123Value", Assert.Contains("{{Qux_123}}", subject.Variables));
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
