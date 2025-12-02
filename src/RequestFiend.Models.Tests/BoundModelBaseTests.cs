using Microsoft.Maui.Controls;
using Xunit;

namespace RequestFiend.Models.Tests;

public class BoundModelBaseTests {
    [Theory]
    [MemberData(nameof(PageWidth_Sets_StackOProperties_Data))]
    public void PageWidth_Sets_StackOProperties(double pageWidth, bool expectedHorizontalFill, StackOrientation expectedStackOrientation, bool expectedStackIsHorizontal, bool expectedStackIsVertical) {
        var subject = new BoundModelBase() {
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
}
