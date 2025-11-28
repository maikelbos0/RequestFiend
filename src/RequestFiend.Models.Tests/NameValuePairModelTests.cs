using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelTests {
    [Fact]
    public void Reinitialize() {
        var subject = new NameValuePairModel(new() { Name = "PreviousName", Value = "PreviousValue" });

        var reinitializedPair = new NameValuePair() {
            Name = "NewName",
            Value = "NewValue"
        };

        subject.Reinitialize(reinitializedPair);

        Assert.Equal(reinitializedPair.Name, subject.Name.Value);
        Assert.False(subject.Name.IsModified);
        Assert.Equal(reinitializedPair.Value, subject.Value.Value);
        Assert.False(subject.Value.IsModified);
    }
}
