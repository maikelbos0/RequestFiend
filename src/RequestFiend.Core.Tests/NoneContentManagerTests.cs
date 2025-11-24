using Xunit;

namespace RequestFiend.Core.Tests;

public class NoneContentManagerTests {
    [Fact]
    public void GetContent() {
        var subject = new NoneContentManager();
        var request = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost",
            StringContent = "Just a piece of text"
        };
        var collection = new RequestTemplateCollection();

        Assert.Null(subject.GetContent(request, collection));
    }
}
