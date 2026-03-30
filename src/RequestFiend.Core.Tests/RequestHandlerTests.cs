using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestHandlerTests {
    [Fact]
    public async Task Execute() {
        var expectedResponse = new HttpResponseMessage();
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(expectedResponse);
        var httpClient = new HttpClient(httpMessageHandler);

        var subject = new RequestHandler(httpClient);

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost/"
        };

        var result = await subject.Execute(request, new(), CancellationToken.None);

        Assert.NotNull(result.Request);
        Assert.NotNull(result.Request.RequestUri);
        Assert.Equal(request.Url, result.Request.RequestUri.ToString());
        Assert.Same(expectedResponse, result.Response);
        Assert.Null(result.Exception);
    }

    [Fact]
    public async Task Execute_Returns_Request_Creation_Exception() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(new HttpResponseMessage());
        var httpClient = new HttpClient(httpMessageHandler);

        var subject = new RequestHandler(httpClient);

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "An invalid URL"
        };

        var result = await subject.Execute(request, new(), CancellationToken.None);

        Assert.Null(result.Request);
        Assert.Null(result.Response);
        Assert.NotNull(result.Exception);
    }

    [Fact]
    public async Task Execute_Returns_Request_Execution_Exception() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        var expectedException = new InvalidOperationException();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Throws(expectedException);
        var httpClient = new HttpClient(httpMessageHandler);

        var subject = new RequestHandler(httpClient);

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost/"
        };

        var result = await subject.Execute(request, new(), CancellationToken.None);

        Assert.NotNull(result.Request);
        Assert.Null(result.Response);
        Assert.Same(expectedException, result.Exception);
    }

    [Fact]
    public async Task Execute_Notifies_Listener_Of_Request() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(new HttpResponseMessage());
        var httpClient = new HttpClient(httpMessageHandler);
        var requestPipelineListener = Substitute.For<IRequestExchangeListener>();

        var subject = new RequestHandler(httpClient);

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost/"
        };

        var result = await subject.Execute(request, new(), requestPipelineListener, CancellationToken.None);

        await requestPipelineListener.Received(1).OnRequestCreated(Arg.Is<HttpRequestMessage>(request => request == result.Request));
    }

    [Fact]
    public async Task Execute_Notifies_Listener_Of_Response() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(new HttpResponseMessage());
        var httpClient = new HttpClient(httpMessageHandler);
        var requestPipelineListener = Substitute.For<IRequestExchangeListener>();

        var subject = new RequestHandler(httpClient);

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost/"
        };

        var result = await subject.Execute(request, new(), requestPipelineListener, CancellationToken.None);

        await requestPipelineListener.Received(1).OnResponseReceived(Arg.Is<HttpResponseMessage>(response => response == result.Response));
    }

    [Fact]
    public async Task Execute_Notifies_Listener_Of_Exception() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Throws(new InvalidOperationException());
        var httpClient = new HttpClient(httpMessageHandler);
        var requestPipelineListener = Substitute.For<IRequestExchangeListener>();

        var subject = new RequestHandler(httpClient);

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost/"
        };

        var result = await subject.Execute(request, new(), requestPipelineListener, CancellationToken.None);

        await requestPipelineListener.Received(1).OnExceptionCaught(Arg.Is<Exception>(exception => exception == result.Exception));
    }
}
