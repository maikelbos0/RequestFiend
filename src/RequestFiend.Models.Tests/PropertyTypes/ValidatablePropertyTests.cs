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
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
    }

    [Theory]
    [InlineData(null, null, true, false, false)]
    [InlineData(null, "", true, true, false)]
    [InlineData(null, "Changed", false, true, true)]
    [InlineData("Initial", "Initial", false, false, false)]
    [InlineData("Initial", "Changed", false, true, true)]
    [InlineData("Initial", null, true, true, false)]
    [InlineData("Initial", "", true, true, false)]
    public void Value(string? initialValue, string? newValue, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError) {
        var subject = new ValidatableProperty<string?>(() => initialValue, value => !string.IsNullOrEmpty(value)) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
    }

    [Fact]
    public void Reset() {
        const string initialValue = "Initial";
        const string resetValue = "Reinitialized";

        var subject = new ValidatableProperty<string?>(() => initialValue);

        subject.Reset(() => resetValue);

        Assert.Equal(resetValue, subject.Value);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
    }
}
