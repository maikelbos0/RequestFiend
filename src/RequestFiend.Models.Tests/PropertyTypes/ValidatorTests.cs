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
    [InlineData("", true)]
    [InlineData(" ", false)]
    [InlineData("Value", false)]
    [InlineData("123", true)]
    public void Numeric(string value, bool expectedResult) {
        Assert.Equal(expectedResult, Validator.Numeric(value));
    }

    [Theory]
    [InlineData("Foo", true)]
    [InlineData("123", true)]
    [InlineData("Да", true)]
    [InlineData("٢٣٤٥٦٧٨٩", true)]
    [InlineData("１２３", true)]
    [InlineData("Foo_132", true)]
    [InlineData("Foo+132", false)]
    [InlineData("Foo 132", false)]
    [InlineData("Foo/132", false)]
    public void VariableName(string value, bool expectedResult) {
        Assert.Equal(expectedResult, Validator.VariableName(value));
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    [InlineData(false, false, true)]
    public void Conditional(bool isRequired, bool isValid, bool expectedResult) {
        var subject = Validator.Conditional(() => isRequired, _ => isValid);

        Assert.Equal(expectedResult, subject("Value"));
    }
}
