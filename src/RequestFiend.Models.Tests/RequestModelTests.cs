using NSubstitute;
using RequestFiend.Core;
using System.Collections.Generic;
using System.ComponentModel;
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

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Exchange", subject.PageTitleBase);
        Assert.Equal("Exchange", subject.ShellItemTitleBase);
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
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();

        var subject = new RequestModel(requestHandler, new(filePath), collection, request);
        subject.PropertyChanged += (_, e) => {
            switch (e.PropertyName) {
                case nameof(RequestModel.PageTitleBase):
                    pageTitleBaseValues.Add(subject.PageTitleBase);
                    break;
                case nameof(RequestModel.ShellItemTitleBase):
                    shellItemTitleBaseValues.Add(subject.ShellItemTitleBase);
                    break;
                case nameof(RequestModel.IsExecuting):
                    isExecutingValues.Add(subject.IsExecuting);
                    break;
                default:
                    break;
            }
        };

        await subject.Execute();

        Assert.Same(requestContext, subject.Context);
        Assert.Equal([$"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Executing request...", $"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Exchange"], pageTitleBaseValues);
        Assert.Equal(["Executing request...", "Exchange"], shellItemTitleBaseValues);
        Assert.Equal([true, false], isExecutingValues);
    }
}
