using NSubstitute;
using RequestFiend.Core;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestModelTests {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestModel(Substitute.For<IRequestHandler>(), new(filePath), collection, request);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Executing...", subject.PageTitleBase);
        Assert.Equal("Executing...", subject.ShellItemTitleBase);
    }

    [Fact]
    public async Task Execute() {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestHandler = Substitute.For<IRequestHandler>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://url"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var requestContext = new RequestContext();
        requestHandler.Execute(request, collection, CancellationToken.None).Returns(requestContext);

        var subject = new RequestModel(requestHandler, new(filePath), collection, request);

        await subject.Execute(CancellationToken.None);

        Assert.Same(requestContext, subject.Context);
        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Response", subject.PageTitleBase);
        Assert.Equal("Response", subject.ShellItemTitleBase);
    }
}
