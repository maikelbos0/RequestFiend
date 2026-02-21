using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests.PropertyTypes;

public class ValidatorTests {
    [Theory]
    [InlineData("", false)]
    [InlineData(" ", true)]
    [InlineData("Value", true)]
    [InlineData("123", true)]
    public void Required(string value, bool expectedResult) {
        Assert.Equal(expectedResult, Validator.Required(value));
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("Value", false)]
    [InlineData("123", true)]
    public void Numeric(string value, bool expectedResult) {
        Assert.Equal(expectedResult, Validator.Numeric(value));
    }
}
