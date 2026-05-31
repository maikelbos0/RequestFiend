using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelTests {
    [Fact]
    public void Constructor() {
        var pair = new NameValuePair() { Name = "Name", Value = "Value" };
        var subject = new NameValuePairModel(pair, Validator.Required);

        Assert.Equal(pair.Name, subject.Name.Value);
        Assert.Equal(pair.Value, subject.Value.Value);
        Assert.Equal([subject.Name, subject.Value], subject.Validatables);
    }

    [Fact]
    public void Reset() {
        var subject = new NameValuePairModel("PreviousName", "PreviousValue", Validator.Required);

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

    [Fact]
    public void GetNameValuePair() {
        var subject = new NameValuePairModel("Name", "Value", Validator.Required);

        var result = subject.GetNameValuePair();

        Assert.Equal(subject.Name.Value, result.Name);
        Assert.Equal(subject.Value.Value, result.Value);
    }
}
