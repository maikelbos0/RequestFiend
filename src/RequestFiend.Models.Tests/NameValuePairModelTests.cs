using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelTests {
    [Fact]
    public void Name() {
        var pair = new NameValuePair() { Name = "PreviousName", Value = "PreviousValue" };

        var subject = new NameValuePairModel(pair, Validator.Required) {
            Name = { Value = "Name" }
        };

        Assert.Equal("Name", pair.Name);
    }

    [Fact]
    public void Value() {
        var pair = new NameValuePair() { Name = "PreviousName", Value = "PreviousValue" };

        var subject = new NameValuePairModel(pair, Validator.Required) {
            Value = { Value = "Value" }
        };

        Assert.Equal("Value", pair.Value);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Constructor(bool isValid, bool expectedHasError) {
        var pair = new NameValuePair() { Name = "Name", Value = "Value" };

        var subject = new NameValuePairModel(pair, _ => isValid, _ => isValid);

        Assert.Equal(pair.Name, subject.Name.Value);
        Assert.Equal(expectedHasError, subject.Name.HasError);
        Assert.Equal(pair.Value, subject.Value.Value);
        Assert.Equal(expectedHasError, subject.Value.HasError);
        Assert.Equal([subject.Name, subject.Value], subject.Validatables);
    }
}
