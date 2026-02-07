using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests.PropertyTypes;

public class ValidatablePropertyTests {
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Value", false)]
    public void Constructor(string? initialValue, bool expectedHasError) {
        var subject = new ValidatableProperty<string?>(() => initialValue, value => !string.IsNullOrEmpty(value));

        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData(null, null, false, true)]
    [InlineData(null, "", false, true)]
    [InlineData(null, "Changed", true, false)]
    [InlineData("Initial", "Initial", false, false)]
    [InlineData("Initial", "Changed", true, false)]
    [InlineData("Initial", null, false, true)]
    [InlineData("Initial", "", false, true)]
    public void Value(string? initialValue, string? newValue, bool expectedIsModified, bool expectedHasError) {
        var subject = new ValidatableProperty<string?>(() => initialValue, value => !string.IsNullOrEmpty(value)) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Fact]
    public void Reset() {
        const string initialValue = "Initial";
        const string resetValue = "Reinitialized";

        var subject = new ValidatableProperty<string?>(() => initialValue);

        subject.Reset(() => resetValue);

        Assert.Equal(resetValue, subject.Value);
        Assert.False(subject.IsModified);
    }
}
