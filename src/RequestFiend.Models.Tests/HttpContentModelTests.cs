using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class HttpContentModelTests {
    [Fact]
    public async Task Create_Null_Content() {
        var subject = await HttpContentModel.Create(null);

        Assert.Equal(HttpContentType.None, subject.Type);
        Assert.Null(subject.TextContent);
        Assert.Null(subject.BinaryContent);
    }

    [Fact]
    public async Task Create_Null_ContentType() {
        var content = new StringContent("Content") { Headers = { ContentType = null } };

        var subject = await HttpContentModel.Create(content);

        Assert.Equal(HttpContentType.Unknown, subject.Type);
        Assert.Null(subject.TextContent);
        Assert.Equal([67, 111, 110, 116, 101, 110, 116], subject.BinaryContent);
    }

    [Theory]
    [InlineData("text/plain", null, HttpContentType.Text, "Content", null)]
    [InlineData("text/xml", null, HttpContentType.Text, "Content", null)]
    [InlineData("application/xml", null, HttpContentType.Text, "Content", null)]
    [InlineData("application/xhtml+xml", null, HttpContentType.Text, "Content", null)]
    [InlineData("application/json", null, HttpContentType.Text, "Content", null)]
    [InlineData("application/xhtml+json", null, HttpContentType.Text, "Content", null)]
    [InlineData("application/octet-stream", "UTF-8", HttpContentType.Text, "Content", null)]
    [InlineData("image/bmp", null, HttpContentType.Image, null, new byte[] { 67, 111, 110, 116, 101, 110, 116 })]
    [InlineData("image/gif", null, HttpContentType.Image, null, new byte[] { 67, 111, 110, 116, 101, 110, 116 })]
    [InlineData("image/jpeg", null, HttpContentType.Image, null, new byte[] { 67, 111, 110, 116, 101, 110, 116 })]
    [InlineData("image/png", null, HttpContentType.Image, null, new byte[] { 67, 111, 110, 116, 101, 110, 116 })]
    [InlineData("image/svg+xml", null, HttpContentType.Image, null, new byte[] { 67, 111, 110, 116, 101, 110, 116 })]
    [InlineData("image/webp", null, HttpContentType.Unknown, null, new byte[] { 67, 111, 110, 116, 101, 110, 116 })]
    [InlineData("application/octet-stream", null, HttpContentType.Unknown, null, new byte[] { 67, 111, 110, 116, 101, 110, 116 })]
    public async Task Create(string mediaType, string? charSet, HttpContentType expectedType, string? expectedTextContent, byte[]? expectedBinaryContent) {
        var content = new StringContent("Content", new MediaTypeHeaderValue(mediaType, charSet));

        var subject = await HttpContentModel.Create(content);

        Assert.Equal(expectedType, subject.Type);
        Assert.Equal(expectedTextContent, subject.TextContent);
        Assert.Equal(expectedBinaryContent, subject.BinaryContent);
    }
}
