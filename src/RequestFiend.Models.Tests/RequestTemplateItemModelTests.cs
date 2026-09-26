using RequestFiend.Core;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateItemModelTests {
    [Fact]
    public void Constructor() {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };

        var subject = new RequestTemplateItemModel(request);

        Assert.Equal(request.Name, subject.Name);
    }

    [Fact]
    public void Equals_Current_Request() {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };

        var subject = new RequestTemplateItemModel(request);

        Assert.True(subject.Equals(request));
    }

    [Fact]
    public void Equals_Different_Request() {
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };

        var subject = new RequestTemplateItemModel(request);

        Assert.False(subject.Equals(new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        }));
    }
}
