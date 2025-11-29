using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelTests {
    [Theory]
    [InlineData("", "", true)]
    [InlineData("", "Value", true)]
    [InlineData("Name", "", true)]
    [InlineData("Name", "Value", false)]
    public void Constructor_HasError(string name, string value, bool expectedResult) {
        var subject = new NameValuePairModel(new() {
            Name = name,
            Value = value
        });

        Assert.Equal(expectedResult, subject.HasError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Name", false)]
    public void Name_HasError(string? name, bool expectedResult) {
        var subject = new NameValuePairModel() {
            Name = { Value = name },
            Value = { Value = "Value" }
        };

        Assert.Equal(expectedResult, subject.HasError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Value", false)]
    public void Value_HasError(string? value, bool expectedResult) {
        var subject = new NameValuePairModel() {
            Name = { Value = "Name" },
            Value = { Value = value }
        };

        Assert.Equal(expectedResult, subject.HasError);
    }

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
