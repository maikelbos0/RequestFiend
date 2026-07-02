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
        var subject = new ValidatableProperty<string>(() => initialValue, value => !string.IsNullOrEmpty(value));

        Assert.Equal(initialValue, subject.Value);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
    }

    [Theory]
    [InlineData("", "", true, false, false, 0)]
    [InlineData("", "Changed", false, true, true, 1)]
    [InlineData("Initial", "Initial", false, false, false, 0)]
    [InlineData("Initial", "Changed", false, true, true, 1)]
    [InlineData("Initial", "", true, true, false, 1)]
    public void Value(string initialValue, string newValue, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError, int expectedUpdaterCalls) {
        var setter = Substitute.For<Action<string>>();
        var subject = new ValidatableProperty<string>(() => initialValue, setter, value => !string.IsNullOrEmpty(value)) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);

        setter.Received(expectedUpdaterCalls).Invoke(newValue);
    }

    [Fact]
    public void Reset() {
        const string value = "Initial";
        var setter = Substitute.For<Action<string>>();

        var subject = new ValidatableProperty<string>(() => value, setter, value => !string.IsNullOrEmpty(value)) {
            Value = ""
        };

        subject.Reset();

        Assert.Equal(value, subject.Value);
        Assert.False(subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        setter.Received(1).Invoke(value);
    }
}
