using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests.PropertyTypes;

public class RequiredStringTests {
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Value", true)]
    public void Constructor(string? initialValue, bool expectedIsValid) {
        var subject = new RequiredString(() => initialValue);

        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
        Assert.Equal(expectedIsValid, subject.IsValid);
    }

    [Theory]
    [InlineData(null, null, false, false)]
    [InlineData(null, "", true, false)]
    [InlineData(null, "Changed", true, true)]
    [InlineData("Initial", "Initial", false, true)]
    [InlineData("Initial", "Changed", true, true)]
    [InlineData("Initial", null, true, false)]
    [InlineData("Initial", "", true, false)]
    public void Value(string? initialValue, string? newValue, bool expectedIsModified, bool expectedIsValid) {
        var subject = new RequiredString(() => initialValue) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedIsModified, subject.IsModified);

        Assert.Equal(expectedIsValid, subject.IsValid);
    }

    [Fact]
    public void Reset() {
        const string initialValue = "Initial";

        var subject = new RequiredString(() => initialValue) {
            Value = "Changed"
        };

        subject.Reset();

        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
    }
}
