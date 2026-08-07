using NSubstitute;
using System.IO.Abstractions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class FileContentManagerTests : TestsBase {
    [Theory]
    [InlineData(false, "application/json")]
    [InlineData(true, null)]
    public async Task GetContent(bool hasManualContentTypeHeader, string? expectedMediaType) {
        var fileContents = Encoding.UTF8.GetBytes("{\"Value\": \"Foo\"}");

        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.ReadAllBytes("./Data.json").Returns(fileContents);

        var subject = new FileContentManager(fileSystem);
        var request = new RequestTemplateSnapshot(
            new([
                new("{{FileName}}", "Data.json")
            ]),
            "Request",
            "POST",
            "https://localhost/",
            [],
            ContentType.File,
            hasManualContentTypeHeader,
            "StringContent",
            "./{{FileName}}",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        var result = Assert.IsType<ByteArrayContent>(subject.GetContent(request));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal(fileContents, await result.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }
}
