using NSubstitute;
using RequestFiend.Models.PropertyTypes;
using System;
using Xunit;

namespace RequestFiend.Models.Tests.PropertyTypes;

public class ValidatablePropertyTests {
    [Theory]
    [InlineData("", true)]
    [InlineData("Value", false)]
    public void Constructor(string initialValue, bool expectedHasError) {
        var subject = new ValidatableProperty<string>(() => initialValue, Substitute.For<Action<string>>(), value => !string.IsNullOrEmpty(value));

        Assert.Equal(initialValue, subject.Value);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
    }

    [Theory]
    [InlineData("", "", true, false, false)]
    [InlineData("", "Changed", false, true, true)]
    [InlineData("Initial", "Initial", false, false, false)]
    [InlineData("Initial", "Changed", false, true, true)]
    [InlineData("Initial", "", true, true, false)]
    public void Value(string initialValue, string newValue, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError) {
        var subject = new ValidatableProperty<string>(() => initialValue, Substitute.For<Action<string>>(), value => !string.IsNullOrEmpty(value)) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
    }

    [Fact]
    public void Set() {
        const string value = "New";
        var setter = Substitute.For<Action<string>>();

        var subject = new ValidatableProperty<string>(() => "Initial", setter, value => !string.IsNullOrEmpty(value)) {
            Value = value
        };

        subject.Set();

        Assert.False(subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal(value, subject.Value);
        setter.Received(1).Invoke(value);
    }

    [Fact]
    public void Reset() {
        const string value = "Initial";

        var subject = new ValidatableProperty<string>(() => value, Substitute.For<Action<string>>(), value => !string.IsNullOrEmpty(value)) {
            Value = ""
        };

        subject.Reset();

        Assert.Equal(value, subject.Value);
        Assert.False(subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
    }
}
