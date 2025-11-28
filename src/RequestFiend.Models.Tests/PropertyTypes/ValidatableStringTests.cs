using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests.PropertyTypes;

public class ValidatableStringTests {
    [Theory]
    [InlineData(true, null, true)]
    [InlineData(true, "", true)]
    [InlineData(true, "Value", false)]
    [InlineData(false, null, false)]
    [InlineData(false, "", false)]
    [InlineData(false, "Value", false)]
    public void Constructor(bool isRequired, string? initialValue, bool expectedHasError) {
        var subject = new ValidatableString(isRequired, () => initialValue);

        Assert.Equal(isRequired, subject.IsRequired);
        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData(true, null, null, false, true)]
    [InlineData(true, null, "", false, true)]
    [InlineData(true, null, "Changed", true, false)]
    [InlineData(true, "Initial", "Initial", false, false)]
    [InlineData(true, "Initial", "Changed", true, false)]
    [InlineData(true, "Initial", null, false, true)]
    [InlineData(true, "Initial", "", false, true)]
    [InlineData(false, null, null, false, false)]
    [InlineData(false, null, "", true, false)]
    [InlineData(false, null, "Changed", true, false)]
    [InlineData(false, "Initial", "Initial", false, false)]
    [InlineData(false, "Initial", "Changed", true, false)]
    [InlineData(false, "Initial", null, true, false)]
    [InlineData(false, "Initial", "", true, false)]
    public void Value(bool isRequired, string? initialValue, string? newValue, bool expectedIsModified, bool expectedHasError) {
        var subject = new ValidatableString(isRequired, () => initialValue) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Fact]
    public void Reset() {
        const string initialValue = "Initial";

        var subject = new ValidatableString(false, () => initialValue) {
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

        var subject = new ValidatableString(false, () => initialValue);

        subject.Reinitialize(() => reinitializedValue);

        Assert.Equal(reinitializedValue, subject.Value);
        Assert.False(subject.IsModified);
    }
}
