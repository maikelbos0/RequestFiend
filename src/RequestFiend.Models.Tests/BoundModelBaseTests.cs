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

    [Theory]
    [InlineData(false, false, false, false, false, false, false, "Page title", "Shell item title")]
    [InlineData(true, false, false, false, true, false, false, "Page title", "Shell item title")]
    [InlineData(false, false, true, false, true, false, false, "Page title", "Shell item title")]
    [InlineData(true, true, false, false, true, true, false, "Page title ●", "Shell item title ●")]
    [InlineData(false, false, true, true, true, true, false, "Page title ●", "Shell item title ●")]
    [InlineData(false, true, false, false, false, true, true, "Page title ●", "Shell item title ●")]
    [InlineData(false, false, false, true, false, true, true, "Page title ●", "Shell item title ●")]
    public void State_Initial(
        bool validateblePropertyHasError,
        bool validatablePropertyIsModified,
        bool nameValuePairPropertyHasError,
        bool nameValuePairPropertyIsModified,
        bool expectedHasError,
        bool expectedIsModified,
        bool expectedIsModifiedWithoutError,
        string expectedPageTitle,
        string expectedShellItemTitle
    ) {
        var validatableProperty = new ValidatableProperty<string>(() => "Name");
        var nameValuePairModelCollection = new NameValuePairModelCollection([new() { Name = "Name" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        validatableProperty.HasError = validateblePropertyHasError;
        validatableProperty.IsModified = validatablePropertyIsModified;
        nameValuePairModelCollection[0].Name.HasError = nameValuePairPropertyHasError;
        nameValuePairModelCollection[0].Name.IsModified = nameValuePairPropertyIsModified;

        subject.ConfigureState([validatableProperty], [nameValuePairModelCollection]);

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }

    [Theory]
    [InlineData(false, false, false, false, false, false, false, "Page title", "Shell item title")]
    [InlineData(true, false, false, false, true, false, false, "Page title", "Shell item title")]
    [InlineData(false, false, true, false, true, false, false, "Page title", "Shell item title")]
    [InlineData(true, true, false, false, true, true, false, "Page title ●", "Shell item title ●")]
    [InlineData(false, false, true, true, true, true, false, "Page title ●", "Shell item title ●")]
    [InlineData(false, true, false, false, false, true, true, "Page title ●", "Shell item title ●")]
    [InlineData(false, false, false, true, false, true, true, "Page title ●", "Shell item title ●")]
    public void State(
        bool validateblePropertyHasError,
        bool validatablePropertyIsModified,
        bool nameValuePairPropertyHasError,
        bool nameValuePairPropertyIsModified,
        bool expectedHasError,
        bool expectedIsModified,
        bool expectedIsModifiedWithoutError,
        string expectedPageTitle,
        string expectedShellItemTitle
    ) {
        var validatableProperty = new ValidatableProperty<string>(() => "Name");
        var nameValuePairModelCollection = new NameValuePairModelCollection([new() { Name = "Name" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatableProperty], [nameValuePairModelCollection]);

        validatableProperty.HasError = validateblePropertyHasError;
        validatableProperty.IsModified = validatablePropertyIsModified;
        nameValuePairModelCollection[0].Name.HasError = nameValuePairPropertyHasError;
        nameValuePairModelCollection[0].Name.IsModified = nameValuePairPropertyIsModified;

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Pair_Is_Added() {
        var nameValuePairModelCollection = new NameValuePairModelCollection([]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([], [nameValuePairModelCollection]);

        nameValuePairModelCollection.Add(new());

        Assert.True(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.False(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }

    [Fact]
    public void State_When_Pair_Is_Removed() {
        var nameValuePairModelCollection = new NameValuePairModelCollection([new() { Name = "Name" }, new() { Name = "Name" }]);

        var subject = new BoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([], [nameValuePairModelCollection]);

        nameValuePairModelCollection.Remove(nameValuePairModelCollection.Last());

        Assert.False(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.True(subject.IsModifiedWithoutError);
        Assert.Equal($"{subject.PageTitleBase} ●", subject.PageTitle);
        Assert.Equal($"{subject.ShellItemTitleBase} ●", subject.ShellItemTitle);
    }
}
