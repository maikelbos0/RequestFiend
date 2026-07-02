using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.Security;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelTests {
    [Fact]
    public void Name() {
        var pair = new NameValuePair() { Name = "PreviousName", Value = "PreviousValue" };

        var subject = new NameValuePairModel(pair, Validator.Required) {
            Name = { Value = "Name" }
        };

        Assert.Equal("Name", pair.Name);
    }

    [Fact]
    public void Value() {
        var pair = new NameValuePair() { Name = "PreviousName", Value = "PreviousValue" };

        var subject = new NameValuePairModel(pair, Validator.Required) {
            Value = { Value = "Value" }
        };

        Assert.Equal("Value", pair.Value);
    }

    [Fact]
    public void Constructor() {
        var pair = new NameValuePair() { Name = "Name", Value = "Value" };

        // TODO refactor to not use public property for validator and remove warning suppression from csproj
        var nameValidator = (string value) => true;
        var valueValidator = (string value) => true;

        var subject = new NameValuePairModel(pair, nameValidator, valueValidator);

        Assert.Equal(pair.Name, subject.Name.Value);
        Assert.Same(nameValidator, subject.Name.Validator);
        Assert.Equal(pair.Value, subject.Value.Value);
        Assert.Same(valueValidator, subject.Value.Validator);
        Assert.Equal([subject.Name, subject.Value], subject.Validatables);
    }

    [Fact]
    public void Reset() {
        var subject = new NameValuePairModel(new NameValuePair() { Name = "PreviousName", Value = "PreviousValue" }, Validator.Required);

        subject.Name.Value = "Name";
        subject.Value.Value = "Name";

        subject.Reset();

        Assert.False(subject.IsModified);
        Assert.False(subject.Name.IsModified);
        Assert.False(subject.Value.IsModified);
    }
}
