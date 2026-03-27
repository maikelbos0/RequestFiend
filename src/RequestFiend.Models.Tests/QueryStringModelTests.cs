using Xunit;

namespace RequestFiend.Models.Tests;

public class QueryStringModelTests {
    [Theory]
    [InlineData("https://localhost/api")]
    [InlineData("https://localhost/api?")]
    public void Constructor_Without_Parameters(string url) {
        var subject = new QueryStringModel(url);

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Empty(subject.Parameters);
        Assert.Equal("https://localhost/api", subject.Url);
    }

    [Fact]
    public void Constructor_With_Parameters() {
        var subject = new QueryStringModel("https://localhost/api?Foo&Bar=Test?&Baz");

        Assert.Equal("https://localhost/api", subject.BaseUrl.Value);
        Assert.Equal(3, subject.Parameters.Count);
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Foo" && pair.Value.Value == "");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Bar" && pair.Value.Value == "Test?");
        Assert.Contains(subject.Parameters, pair => pair.Name.Value == "Baz" && pair.Value.Value == "");
        Assert.Equal("https://localhost/api?Foo=&Bar=Test?&Baz=", subject.Url);
    }
}
