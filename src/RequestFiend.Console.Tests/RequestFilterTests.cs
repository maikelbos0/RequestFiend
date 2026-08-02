using System.Text.RegularExpressions;
using Xunit;

namespace RequestFiend.Console.Tests;

public class RequestFilterTests {
    [Theory]
    [InlineData(null, null, "Foo", true)]
    [InlineData("^foo$", null, "Foo", true)]
    [InlineData("^bar$", null, "Foo", false)]
    [InlineData(null, "^bar$", "Foo", true)]
    [InlineData(null, "^foo$", "Foo", false)]
    [InlineData("^foo$", "^bar$", "Foo", true)]
    [InlineData("^foo$", "^foo$", "Foo", false)]
    [InlineData("^bar", "^bar$", "Foo", false)]
    [InlineData("^bar", "^foo$", "Foo", false)]
    public void IsMatch(string? include, string? exclude, string requestName, bool expectedResult) {
        var subject = new RequestFilter(
            include == null ? null : new Regex(include, RegexOptions.IgnoreCase),
            exclude == null ? null : new Regex(exclude, RegexOptions.IgnoreCase)
        );

        var result = subject.IsMatch(new() {
            Name = requestName,
            Method = "GET",
            Url = "https://localhost"
        });

        Assert.Equal(expectedResult, result);
    }
}
