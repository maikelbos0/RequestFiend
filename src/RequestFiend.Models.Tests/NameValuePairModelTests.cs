using Newtonsoft.Json.Linq;
using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelTests {
    [Theory]
    [InlineData("", "", true)]
    [InlineData("", "Value", true)]
    [InlineData("Name", "", true)]
    [InlineData("Name", "Value", false)]
    public void Constructor_NameValuePair(string name, string value, bool expectedHasError) {
        var subject = new NameValuePairModel(new() {
            Name = name,
            Value = value
        });

        Assert.False(subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Fact]
    public void Constructor_Empty() {
        var subject = new NameValuePairModel();

        Assert.True(subject.HasError);
        Assert.False(subject.IsModified);
    }

    [Theory]
    [InlineData("", false, true)]
    [InlineData("Name", true, false)]
    public void Name(string? name, bool expectedIsModified, bool expectedHasError) {
        var subject = new NameValuePairModel(new() {
            Name = "",
            Value = "Value"
        }) {
            Name = {
                Value = name
            }
        };

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData("", false, true)]
    [InlineData("Value", true, false)]
    public void Value(string? value, bool expectedIsModified, bool expectedHasError) {
        var subject = new NameValuePairModel(new() {
            Name = "Name",
            Value = ""
        }) {
            Value = {
                Value = value
            }
        };

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasError, subject.HasError);
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
        Assert.False(subject.IsModified);
    }
}
