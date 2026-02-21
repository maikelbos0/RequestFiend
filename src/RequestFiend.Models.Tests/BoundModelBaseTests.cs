using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;
using System.Linq;
using Xunit;

namespace RequestFiend.Models.Tests;

public class BoundModelBaseTests {
    [Theory]
    [MemberData(nameof(PageWidth_Sets_StackOProperties_Data))]
    public void PageWidth_Sets_StackOProperties(double pageWidth, bool expectedHorizontalFill, StackOrientation expectedStackOrientation, bool expectedStackIsHorizontal, bool expectedStackIsVertical) {
        var subject = new BoundModelBase("Page title", "Shell item title") {
            PageWidth = pageWidth
        };

        var expectedStackHorizontalOptions = expectedHorizontalFill ? LayoutOptions.Fill : LayoutOptions.End;

        Assert.Equal(expectedStackHorizontalOptions, subject.StackHorizontalOptions);
        Assert.Equal(expectedStackOrientation, subject.StackOrientation);
        Assert.Equal(expectedStackIsHorizontal, subject.StackIsHorizontal);
        Assert.Equal(expectedStackIsVertical, subject.StackIsVertical);
    }

    public static TheoryData<double, bool, StackOrientation, bool, bool> PageWidth_Sets_StackOProperties_Data()
        => new() {
            { 400, true, StackOrientation.Vertical, false, true },
            { 674, true, StackOrientation.Vertical, false, true },
            { 675, false, StackOrientation.Horizontal, true, false },
            { 900, false, StackOrientation.Horizontal, true, false },
        };

    [Fact]
    public void Constructor() {
        const string pageTitleBase = "Page title";
        const string shellItemTitleBase = "Shell item title";

        var subject = new BoundModelBase(pageTitleBase, shellItemTitleBase);

        Assert.Equal(pageTitleBase, subject.PageTitleBase);
        Assert.Equal(pageTitleBase, subject.PageTitle);
        Assert.Equal(shellItemTitleBase, subject.ShellItemTitleBase);
        Assert.Equal(shellItemTitleBase, subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Nothing_Is_Modified() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        Assert.False(subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal(subject.PageTitleBase, subject.PageTitle);
        Assert.Equal(subject.ShellItemTitleBase, subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Nothing_Is_Modified_With_Property_Error() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value", Validator.Numeric);
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        Assert.True(subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal(subject.PageTitleBase, subject.PageTitle);
        Assert.Equal(subject.ShellItemTitleBase, subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Nothing_Is_Modified_With_Pair_Name_Error() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        Assert.True(subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal(subject.PageTitleBase, subject.PageTitle);
        Assert.Equal(subject.ShellItemTitleBase, subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Nothing_Is_Modified_With_Pair_Value_Error() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        Assert.True(subject.HasError);
        Assert.False(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal(subject.PageTitleBase, subject.PageTitle);
        Assert.Equal(subject.ShellItemTitleBase, subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Property_Is_Modified() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        validatableProperty2.Value = "Changed";

        Assert.False(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.True(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Pair_Name_Is_Modified() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        nameValuePairModelCollection2[1].Name.Value = "Changed";

        Assert.False(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.True(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Pair_Value_Is_Modified() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        nameValuePairModelCollection2[1].Value.Value = "Changed";

        Assert.False(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.True(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }
    
    [Fact]
    public void State_When_Property_Has_Error() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value", Validator.Numeric);
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        validatableProperty1.Value = "";

        Assert.True(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Pair_Name_Has_Error() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        nameValuePairModelCollection2[1].Name.Value = "";

        Assert.True(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Pair_Value_Has_Error() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        nameValuePairModelCollection2[1].Value.Value = "";

        Assert.True(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Pair_Is_Added() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        nameValuePairModelCollection2.Add(new());

        Assert.True(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Pair_Is_Removed() {
        var validatableProperty1 = new ValidatableProperty<string>(() => "Name");
        var validatableProperty2 = new ValidatableProperty<string>(() => "Value");
        var nameValuePairModelCollection1 = new NameValuePairModelCollection([new() { Name = "FirstName", Value = "FirstValue" }, new() { Name = "SecondName", Value = "SecondValue" }]);
        var nameValuePairModelCollection2 = new NameValuePairModelCollection([new() { Name = "ThirdName", Value = "ThirdValue" }, new() { Name = "FourthName", Value = "FourthValue" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty1, validatableProperty2], [nameValuePairModelCollection1, nameValuePairModelCollection2]);

        nameValuePairModelCollection2.Remove(nameValuePairModelCollection2.Last());

        Assert.False(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.True(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }
}
