using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests.PropertyTypes;

public class ValidatableStringTests {
    [Theory]
    [InlineData(ValidationMode.None, null, false)]
    [InlineData(ValidationMode.None, "", false)]
    [InlineData(ValidationMode.None, "Value", false)]
    [InlineData(ValidationMode.Required, null, true)]
    [InlineData(ValidationMode.Required, "", true)]
    [InlineData(ValidationMode.Required, "Value", false)]
    [InlineData(ValidationMode.Numeric, null, true)]
    [InlineData(ValidationMode.Numeric, "", true)]
    [InlineData(ValidationMode.Numeric, "Value", true)]
    [InlineData(ValidationMode.Numeric, "123", false)]
    public void Constructor(ValidationMode mode, string? initialValue, bool expectedHasError) {
        var subject = new ValidatableString(mode, () => initialValue);

        Assert.Equal(mode, subject.Mode);
        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData(ValidationMode.None, null, null, false, false)]
    [InlineData(ValidationMode.None, null, "", true, false)]
    [InlineData(ValidationMode.None, null, "Changed", true, false)]
    [InlineData(ValidationMode.None, "Initial", "Initial", false, false)]
    [InlineData(ValidationMode.None, "Initial", "Changed", true, false)]
    [InlineData(ValidationMode.None, "Initial", null, true, false)]
    [InlineData(ValidationMode.None, "Initial", "", true, false)]
    [InlineData(ValidationMode.Required, null, null, false, true)]
    [InlineData(ValidationMode.Required, null, "", false, true)]
    [InlineData(ValidationMode.Required, null, "Changed", true, false)]
    [InlineData(ValidationMode.Required, "Initial", "Initial", false, false)]
    [InlineData(ValidationMode.Required, "Initial", "Changed", true, false)]
    [InlineData(ValidationMode.Required, "Initial", null, false, true)]
    [InlineData(ValidationMode.Required, "Initial", "", false, true)]
    [InlineData(ValidationMode.Numeric, null, null, false, true)]
    [InlineData(ValidationMode.Numeric, null, "", false, true)]
    [InlineData(ValidationMode.Numeric, null, "Changed", false, true)]
    [InlineData(ValidationMode.Numeric, "Initial", "Initial", false, true)]
    [InlineData(ValidationMode.Numeric, "Initial", "Changed", false, true)]
    [InlineData(ValidationMode.Numeric, "Initial", null, false, true)]
    [InlineData(ValidationMode.Numeric, "Initial", "", false, true)]
    [InlineData(ValidationMode.Numeric, "123", "456", true, false)]
    [InlineData(ValidationMode.Numeric, "123", "123", false, false)]
    public void Value(ValidationMode mode, string? initialValue, string? newValue, bool expectedIsModified, bool expectedHasError) {
        var subject = new ValidatableString(mode, () => initialValue) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Fact]
    public void Reset() {
        const string initialValue = "Initial";

        var subject = new ValidatableString(ValidationMode.None, () => initialValue) {
            Value = "Changed"
        };

        subject.Reset();

        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
    }

    [Fact]
    public void Reinitialize() {
        const string initialValue = "Initial";
        const string reinitializedValue = "Reinitialized";

        var subject = new ValidatableString(ValidationMode.None, () => initialValue);

        subject.Reinitialize(() => reinitializedValue);

        Assert.Equal(reinitializedValue, subject.Value);
        Assert.False(subject.IsModified);
    }
}
