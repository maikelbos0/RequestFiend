using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelTests {
    [Theory]
    [InlineData("", true)]
    [InlineData("Name", false)]
    public void Constructor(string name, bool expectedHasError) {
        var pair = new NameValuePair() { Name = name, Value = "Value" };
        var subject = new NameValuePairModel(pair);

        Assert.Equal(pair.Name, subject.Name.Value);
        Assert.Equal(pair.Value, subject.Value.Value);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.False(subject.IsModified);
    }

    [Theory]
    [InlineData(false, false, false, false, false, false)]
    [InlineData(true, false, false, false, true, false)]
    [InlineData(false, false, true, false, true, false)]
    [InlineData(false, true, false, false, false, true)]
    [InlineData(false, false, false, true, false, true)]
    public void State(bool nameHasError, bool nameIsModified, bool valueHasError, bool valueIsModified, bool expectedHasError, bool expectedIsModified) {
        var subject = new NameValuePairModel(new() { Name = "Name", Value = "Value" }) {
            Name = { HasError = nameHasError, IsModified = nameIsModified },
            Value = { HasError = valueHasError, IsModified = valueIsModified }
        };

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
    }

    [Fact]
    public void Reset() {
        var subject = new NameValuePairModel(new() { Name = "PreviousName", Value = "PreviousValue" });

        var resetPair = new NameValuePair() {
            Name = "NewName",
            Value = "NewValue"
        };

        subject.Reset(resetPair);

        Assert.Equal(resetPair.Name, subject.Name.Value);
        Assert.False(subject.Name.IsModified);
        Assert.Equal(resetPair.Value, subject.Value.Value);
        Assert.False(subject.Value.IsModified);
    }
}
