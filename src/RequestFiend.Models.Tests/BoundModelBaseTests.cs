using RequestFiend.Models.PropertyTypes;
using Xunit;

namespace RequestFiend.Models.Tests;

public class BoundModelBaseTests {
    [Theory]
    [InlineData(false, false, false, false, false)]
    [InlineData(true, false, true, false, false)]
    [InlineData(true, true, true, true, false)]
    [InlineData(false, true, false, true, true)]
    public void ConfigureState(bool validateblePropertyHasError, bool validatablePropertyIsModified, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError) {
        var validatable = new ValidatableProperty<string>(() => "Name");
        var subject = new BoundModelBase();

        validatable.HasError = validateblePropertyHasError;
        validatable.IsModified = validatablePropertyIsModified;

        subject.ConfigureState([validatable]);

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
    }

    [Theory]
    [InlineData(false, false, false, false, false)]
    [InlineData(true, false, true, false, false)]
    [InlineData(true, true, true, true, false)]
    [InlineData(false, true, false, true, true)]
    public void State(bool validateblePropertyHasError, bool validatablePropertyIsModified, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError) {
        var validatable = new ValidatableProperty<string>(() => "Name");

        var subject = new BoundModelBase();

        subject.ConfigureState([validatable]);

        validatable.HasError = validateblePropertyHasError;
        validatable.IsModified = validatablePropertyIsModified;

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
    }
}
