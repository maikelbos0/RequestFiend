using Xunit;

namespace RequestFiend.Console.Tests;

public class GlobParserTests {
    [Theory]
    [InlineData("Test", true, "Test")]
    [InlineData("*Test", true, ".*Test")]
    [InlineData("Test?", true, "Test.")]
    [InlineData(". $ ^ { ( | ) + \\", true, "\\. \\$ \\^ \\{ \\( \\| \\) \\+ \\\\")]
    public void TryParse(string input, bool expectedResult, string? expectedPattern) {
        var subject = new GlobParser();

        var result = subject.TryParse(input, out var pattern);

        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedPattern, pattern);
    }
}
