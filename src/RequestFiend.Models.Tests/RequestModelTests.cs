using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestModel(Substitute.For<IMessageService>(), Substitute.For<IRequestHandler>(), new(filePath), collection, request);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Exchange", subject.PageTitleBase);
        Assert.Equal($"{request.Name} - Exchange", subject.ShellItemTitleBase);
    }

    [Fact]
    public async Task Execute() {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestHandler = Substitute.For<IRequestHandler>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();

        var subject = new RequestModel(Substitute.For<IMessageService>(), requestHandler, new(filePath), collection, request);
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

        _ = requestHandler.Received().Execute(request, collection, subject, Arg.Any<CancellationToken>());
        Assert.Equal([$"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Executing request...", $"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Exchange"], pageTitleBaseValues);
        Assert.Equal([$"{request.Name} - Executing request...", $"{request.Name} - Exchange"], shellItemTitleBaseValues);
        Assert.Equal([true, false], isExecutingValues);
    }

    [Fact]
    public void Close() {
        const string filePath = @"C:\Documents\External data requests.json";

        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestModel(messageService, Substitute.For<IRequestHandler>(), new(filePath), collection, request);

        subject.Close();

        messageService.Received().Send(Arg.Any<CloseRequestMessage>(), subject.Id);
    }

    [Fact]
    public async Task OnRequestCreated() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();
        var expectedRequest = new HttpRequestMessage();

        var subject = new RequestModel(Substitute.For<IMessageService>(), Substitute.For<IRequestHandler>(), new(filePath), collection, request);

        await subject.OnRequestCreated(expectedRequest);

        Assert.NotNull(subject.Request);
    }

    [Fact]
    public async Task OnResponseReceived() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();
        var expectedResponse = new HttpResponseMessage();

        var subject = new RequestModel(Substitute.For<IMessageService>(), Substitute.For<IRequestHandler>(), new(filePath), collection, request);

        await subject.OnResponseReceived(expectedResponse);

        Assert.NotNull(subject.Response);
    }

    [Fact]
    public async Task OnExceptionCaught() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();
        var expectedException = new Exception();

        var subject = new RequestModel(Substitute.For<IMessageService>(), Substitute.For<IRequestHandler>(), new(filePath), collection, request);

        await subject.OnExceptionCaught(expectedException);

        Assert.NotNull(subject.Exception);
    }
}
