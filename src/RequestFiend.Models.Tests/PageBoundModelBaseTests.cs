using Microsoft.Maui.Controls;
using NSubstitute;
using System.ComponentModel;
using Xunit;

namespace RequestFiend.Models.Tests;

public class PageBoundModelBaseTests {
    [Theory]
    [InlineData(400, true, false, false, true)]
    [InlineData(539, true, false, false, true)]
    [InlineData(540, false, true, true, false)]
    [InlineData(900, false, true, true, false)]
    public void PageWidth(
        double pageWidth,
        bool expectedHorizontalFill,
        bool expectedHorizontalStackOrientation,
        bool expectedStackIsHorizontal,
        bool expectedStackIsVertical
    ) {
        var subject = new PageBoundModelBase("Page title", "Shell item title") {
            PageWidth = pageWidth
        };

        var expectedStackHorizontalOptions = expectedHorizontalFill ? LayoutOptions.Fill : LayoutOptions.End;
        var expectedStackOrientation = expectedHorizontalStackOrientation ? StackOrientation.Horizontal : StackOrientation.Vertical;

        Assert.Equal(expectedStackHorizontalOptions, subject.StackHorizontalOptions);
        Assert.Equal(expectedStackOrientation, subject.StackOrientation);
        Assert.Equal(expectedStackIsHorizontal, subject.StackIsHorizontal);
        Assert.Equal(expectedStackIsVertical, subject.StackIsVertical);
    }

    [Fact]
    public void Constructor() {
        const string pageTitleBase = "Page title";
        const string shellItemTitleBase = "Shell item title";

        var subject = new PageBoundModelBase(pageTitleBase, shellItemTitleBase);

        Assert.Equal(pageTitleBase, subject.PageTitleBase);
        Assert.Equal(pageTitleBase, subject.PageTitle);
        Assert.Equal(shellItemTitleBase, subject.ShellItemTitleBase);
        Assert.Equal(shellItemTitleBase, subject.ShellItemTitle);
    }

    [Theory]
    [InlineData(false, false, false, false, false, "Page title", "Shell item title")]
    [InlineData(true, false, true, false, false, "Page title", "Shell item title")]
    [InlineData(true, true, true, true, false, "Page title ●", "Shell item title ●")]
    [InlineData(false, true, false, true, true, "Page title ●", "Shell item title ●")]
    public void ConfigureState(
        bool validateblePropertyHasError,
        bool validatablePropertyIsModified,
        bool expectedHasError,
        bool expectedIsModified,
        bool expectedIsModifiedWithoutError,
        string expectedPageTitle,
        string expectedShellItemTitle
    ) {
        var validatable = Substitute.For<IValidatable>();
        var subject = new PageBoundModelBase("Page title", "Shell item title");

        validatable.HasError.Returns(validateblePropertyHasError);
        validatable.IsModified.Returns(validatablePropertyIsModified);

        subject.ConfigureState([validatable]);

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }

    [Theory]
    [InlineData(false, false, nameof(IValidatable.HasError), false, false, false, "Page title", "Shell item title")]
    [InlineData(true, false, nameof(IValidatable.HasError), true, false, false, "Page title", "Shell item title")]
    [InlineData(true, true, nameof(IValidatable.HasError), true, true, false, "Page title ●", "Shell item title ●")]
    [InlineData(false, true, nameof(IValidatable.HasError), false, true, true, "Page title ●", "Shell item title ●")]
    [InlineData(false, false, nameof(IValidatable.IsModified), false, false, false, "Page title", "Shell item title")]
    [InlineData(true, false, nameof(IValidatable.IsModified), true, false, false, "Page title", "Shell item title")]
    [InlineData(true, true, nameof(IValidatable.IsModified), true, true, false, "Page title ●", "Shell item title ●")]
    [InlineData(false, true, nameof(IValidatable.IsModified), false, true, true, "Page title ●", "Shell item title ●")]
    public void State(
        bool validateblePropertyHasError,
        bool validatablePropertyIsModified,
        string propertyName,
        bool expectedHasError,
        bool expectedIsModified,
        bool expectedIsModifiedWithoutError,
        string expectedPageTitle,
        string expectedShellItemTitle
    ) {
        var validatable = Substitute.For<IValidatable>();

        var subject = new PageBoundModelBase("Page title", "Shell item title");

        subject.ConfigureState([validatable]);

        validatable.HasError.Returns(validateblePropertyHasError);
        validatable.IsModified.Returns(validatablePropertyIsModified);
        validatable.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(propertyName));

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }
}
