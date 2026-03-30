using RequestFiend.Models.Services;
using Xunit;

namespace RequestFiend.Models.Tests.Services;

public class VariableServiceTests {
    [Theory]
    [InlineData("")]
    [InlineData("Foo", "text:Foo")]
    [InlineData("Foo+Bar", "text:Foo+Bar")]
    [InlineData("{Bar}", "text:{Bar}")]
    [InlineData("{Bar}}", "text:{Bar}}")]
    [InlineData("{{Bar}", "text:{{Bar}")]
    [InlineData("{{Bar}}", "variable:{{Bar}}")]
    [InlineData("Foo+{{Bar}}+Baz", "text:Foo+", "variable:{{Bar}}" , "text:+Baz")]
    [InlineData("{{{Bar}}}", "text:{", "variable:{{Bar}}", "text:}")]
    public void EncodeUrlComponent(string text, params string[] expectedResult) {
        var result = VariableService.ProcessText(text, text => $"text:{text}", variableReference => $"variable:{variableReference}");

        Assert.Equal(expectedResult, result);
    }
}
