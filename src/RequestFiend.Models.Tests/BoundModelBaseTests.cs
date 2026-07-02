using NSubstitute;
using System.ComponentModel;
using Xunit;

namespace RequestFiend.Models.Tests;

public class BoundModelBaseTests {
    [Theory]
    [InlineData(false, false, false, false, false)]
    [InlineData(true, false, true, false, false)]
    [InlineData(true, true, true, true, false)]
    [InlineData(false, true, false, true, true)]
    public void ConfigureState(bool validateblePropertyHasError, bool validatablePropertyIsModified, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError) {
        var validatable = Substitute.For<IValidatable>();
        var subject = new BoundModelBase();

        validatable.HasError.Returns(validateblePropertyHasError);
        validatable.IsModified.Returns(validatablePropertyIsModified);

        subject.ConfigureState([validatable]);

        Assert.Equal([validatable], subject.Validatables);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
    }

    [Theory]
    [InlineData(false, false, nameof(IValidatable.HasError), false, false, false)]
    [InlineData(true, false, nameof(IValidatable.HasError), true, false, false)]
    [InlineData(true, true, nameof(IValidatable.HasError), true, true, false)]
    [InlineData(false, true, nameof(IValidatable.HasError), false, true, true)]
    [InlineData(false, false, nameof(IValidatable.IsModified), false, false, false)]
    [InlineData(true, false, nameof(IValidatable.IsModified), true, false, false)]
    [InlineData(true, true, nameof(IValidatable.IsModified), true, true, false)]
    [InlineData(false, true, nameof(IValidatable.IsModified), false, true, true)]
    public void State(bool validateblePropertyHasError, bool validatablePropertyIsModified, string propertyName, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError) {
        var validatable = Substitute.For<IValidatable>();

        var subject = new BoundModelBase();

        subject.ConfigureState([validatable]);

        validatable.HasError.Returns(validateblePropertyHasError);
        validatable.IsModified.Returns(validatablePropertyIsModified);
        validatable.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(propertyName));

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
    }
}
