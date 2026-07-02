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
        var updater = Substitute.For<Action<string>>();
        var subject = new ValidatableProperty<string>(() => initialValue, updater, value => !string.IsNullOrEmpty(value)) {
            Value = newValue
        };

        Assert.Equal(newValue, subject.Value);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);

        updater.Received(expectedUpdaterCalls).Invoke(newValue);
    }

    [Fact]
    public void Reset() {
        const string value = "Initial";
        var updater = Substitute.For<Action<string>>();

        var subject = new ValidatableProperty<string>(() => value, updater, value => !string.IsNullOrEmpty(value)) {
            Value = ""
        };

        subject.Reset();

        Assert.Equal(value, subject.Value);
        Assert.True(subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        updater.Received(1).Invoke(value);
    }
}
