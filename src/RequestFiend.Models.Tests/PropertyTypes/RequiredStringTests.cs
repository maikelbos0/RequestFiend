using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests.PropertyTypes;

public class RequiredStringTests {
    [Fact]
    public void Constructor() {
        const string initialValue = "Initial";

        var subject = new RequiredString(() => initialValue);

        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
    }

    [Theory]
    [InlineData(null, null, false)]
    [InlineData(null, "Changed", true)]
    [InlineData("Initial", "Initial", false)]
    [InlineData("Initial", "Changed", true)]
    [InlineData("Initial", null, true)]
    public void Value_And_IsModified(string? initialValue, string? newValue, bool expectedIsModified) {
        var subject = new RequiredString(() => initialValue);

        subject.Value = newValue;

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedIsModified, subject.IsModified);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("Value", true)]
    public void IsValid(string? value, bool expectedIsValid) {
        var subject = new RequiredString();

        subject.Value = value;

        Assert.Equal(expectedIsValid, subject.IsValid);
    }

    [Fact]
    public void Reset() {
        const string initialValue = "Initial";

        var subject = new RequiredString(() => initialValue);

        subject.Value = "Changed";
        subject.Reset();

        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
    }
}
