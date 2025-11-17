using Microsoft.Maui.Controls;
using Xunit;

namespace RequestFiend.Models.Tests;

public class BoundModelBaseTests {
    [Theory]
    [MemberData(nameof(PageWidth_Sets_StackOProperties_Data))]
    public void PageWidth_Sets_StackOProperties(double pageWidth, LayoutOptions expectedStackHorizontalOptions, StackOrientation expectedStackOrientation) {
        var subject = new BoundModelBase() {
            PageWidth = pageWidth
        };

        Assert.Equal(expectedStackHorizontalOptions, subject.StackHorizontalOptions);
        Assert.Equal(expectedStackOrientation, subject.StackOrientation);
    }

    public static TheoryData<double, LayoutOptions, StackOrientation> PageWidth_Sets_StackOProperties_Data()
        => new() {
            { 400, LayoutOptions.Fill, StackOrientation.Vertical },
            { 674, LayoutOptions.Fill, StackOrientation.Vertical },
            { 675, LayoutOptions.End, StackOrientation.Horizontal },
            { 900, LayoutOptions.End, StackOrientation.Horizontal },
        };
}
