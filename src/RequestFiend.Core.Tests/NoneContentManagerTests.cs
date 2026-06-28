using Xunit;

namespace RequestFiend.Core.Tests;

public class NoneContentManagerTests {
    [Fact]
    public void GetContent() {
        var subject = new NoneContentManager();
        var request = new RequestTemplateSnapshot(
            new([]),
            "Request",
            "GET",
            "https://localhost/",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "Code"),
            new([], "Code"),
            new([], "Code")
        );

        Assert.Null(subject.GetContent(request));
    }
}
