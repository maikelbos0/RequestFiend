using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests.PropertyTypes;

public class OptionalStringTests {
    [Fact]
    public void Constructor() {
        const string initialValue = "Initial";

        var subject = new OptionalString(() => initialValue);

        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
    }

    [Theory]
    [InlineData(null, null, false)]
    [InlineData(null, "Changed", true)]
    [InlineData("Initial", "Initial", false)]
    [InlineData("Initial", "Changed", true)]
    [InlineData("Initial", null, true)]
    public void Value(string? initialValue, string? newValue, bool expectedIsModified) {
        var subject = new OptionalString(() => initialValue) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedIsModified, subject.IsModified);
    }

    [Fact]
    public void Reset() {
        const string initialValue = "Initial";

        var subject = new OptionalString(() => initialValue) {
            Value = "Changed"
        };

        subject.Reset();

        Assert.Equal(initialValue, subject.Value);
        Assert.False(subject.IsModified);
    }
}
