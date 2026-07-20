using Microsoft.Maui.Controls;
using RequestFiend.Models.Converters;
using Xunit;

namespace RequestFiend.Models.Tests.Converters;

public class SelectedItemStyleConverterTests {
    [Fact]
    public void Convert_Returns_Style_If_Equal() {
        var subject = new SelectedItemStyleConverter();
        var style = new Style(typeof(Label));

        var result = subject.Convert([42, 42, style], default!, default!, default!);

        Assert.Equal(style, result);
    }

    [Fact]
    public void Convert_Returns_Null_If_Unequal() {
        var subject = new SelectedItemStyleConverter();
        var style = new Style(typeof(Label));

        var result = subject.Convert([42, 41, style], default!, default!, default!);

        Assert.Null(result);
    }


    [Fact]
    public void Convert_Returns_Null_With_Invalid_Style() {
        var subject = new SelectedItemStyleConverter();
        var style = "LabelStyle";

        var result = subject.Convert([42, 42, style], default!, default!, default!);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_Returns_Null_If_Too_Few_Arguments() {
        var subject = new SelectedItemStyleConverter();

        var result = subject.Convert([42, 42], default!, default!, default!);

        Assert.Null(result);
    }
}
