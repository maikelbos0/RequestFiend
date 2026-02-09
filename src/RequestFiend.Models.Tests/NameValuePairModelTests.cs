using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelTests {
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
