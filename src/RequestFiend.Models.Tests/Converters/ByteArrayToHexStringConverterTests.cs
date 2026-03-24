using RequestFiend.Models.Converters;
using Xunit;

namespace RequestFiend.Models.Tests.Converters;

public class ByteArrayToHexStringConverterTests {
    [Theory]
    [InlineData(null, null)]
    [InlineData(new byte[] { }, "")]
    [InlineData(new byte[] { 0 }, "00")]
    [InlineData(new byte[] { 0, 255 }, "00FF")]
    [InlineData(new byte[] { 0, 1, 2, 3 }, "00010203")]
    [InlineData(new byte[] { 0, 1, 2, 3, 4 }, "00010203 04")]
    [InlineData(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }, "00010203 04050607")]
    [InlineData(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, "00010203 04050607 08")]
    public void Convert(byte[]? binaryContent, string? expectedResult) {
        var subject = new ByteArrayToHexStringConverter();

        var result = subject.Convert(binaryContent, default!, default!, default!);

        Assert.Equal(expectedResult, result);
    }
}
