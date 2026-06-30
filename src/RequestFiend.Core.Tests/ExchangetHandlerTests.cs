using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class ExchangetHandlerTests {
    [Fact]
    public async Task Execute() {
        var expectedResponse = new HttpResponseMessage();
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(expectedResponse);
        var httpClient = new HttpClient(httpMessageHandler);
        var serverCertificateValidationHandler = Substitute.For<IServerCertificateValidationHandler>();

        var subject = new ExchangeHandler(httpClient, Substitute.For<IScriptEvaluator>(), serverCertificateValidationHandler, Substitute.For<ILoggerFactory>());

        var request = new RequestTemplateSnapshot(
            new([]),
            "Name",
            "GET",
            "https://localhost/",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "PreExchangeScript"),
            new([], "PostExchangeScript"),
            new([], "OnExceptionScript")
        );
        var collection = new RequestTemplateCollection();

        var result = await subject.Execute(request, collection, new(false, null, null), CancellationToken.None);

        Received.InOrder(async () => {
            serverCertificateValidationHandler.Initialize(collection);
            await httpMessageHandler.SendAsyncCore(Arg.Is<HttpRequestMessage>(request => request == result.Request), Arg.Any<CancellationToken>());
        });

        Assert.Same(result.SessionData, collection.GetSessionData());
        Assert.Same(result.SessionVariables, collection.GetSessionVariables());
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

        var subject = new ExchangeHandler(httpClient, Substitute.For<IScriptEvaluator>(), Substitute.For<IServerCertificateValidationHandler>(), Substitute.For<ILoggerFactory>());

        var request = new RequestTemplateSnapshot(
            new([]),
            "Name",
            "GET",
            "An invalid URL",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "PreExchangeScript"),
            new([], "PostExchangeScript"),
            new([], "OnExceptionScript")
        );

        var result = await subject.Execute(request, new(), new(false, null, null), CancellationToken.None);

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

        var subject = new ExchangeHandler(httpClient, Substitute.For<IScriptEvaluator>(), Substitute.For<IServerCertificateValidationHandler>(), Substitute.For<ILoggerFactory>());

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost/"
        };

        var result = await subject.Execute(request, new(), new(false, null, null), CancellationToken.None);

        Assert.NotNull(result.Request);
        Assert.Null(result.Response);
        Assert.Same(expectedException, result.Exception);
    }

    [Fact]
    public async Task Execute_Notifies_And_Evaluates_Script_For_Succesful_Response() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(new HttpResponseMessage());
        var httpClient = new HttpClient(httpMessageHandler);
        var scriptEvaluator = Substitute.For<IScriptEvaluator>();
        var exchangeListener = Substitute.For<IExchangeListener>();

        var subject = new ExchangeHandler(httpClient, scriptEvaluator, Substitute.For<IServerCertificateValidationHandler>(), Substitute.For<ILoggerFactory>());

        var request = new RequestTemplateSnapshot(
            new([]),
            "Name",
            "GET",
            "https://localhost/",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "PreExchangeScript"),
            new([], "PostExchangeScript"),
            new([], "OnExceptionScript")
        );

        var result = await subject.Execute(request, new(), new(true, null, null), exchangeListener, CancellationToken.None);

        Received.InOrder(async () => {
            await scriptEvaluator.Evaluate(request.PreExchangeScript, result, CancellationToken.None);
            await exchangeListener.OnRequestCreated(Arg.Is<HttpRequestMessage>(request => request == result.Request));
            await httpMessageHandler.Received().SendAsyncCore(Arg.Is<HttpRequestMessage>(request => request == result.Request), Arg.Any<CancellationToken>());
            await exchangeListener.Received().OnRequestElapsed(Arg.Any<TimeSpan>());
            await scriptEvaluator.Evaluate(request.PostExchangeScript, result, CancellationToken.None);
            await exchangeListener.OnResponseReceived(Arg.Is<HttpResponseMessage>(response => response == result.Response));
        });

        await scriptEvaluator.DidNotReceive().Evaluate(request.OnExceptionScript, Arg.Any<ExchangeContext>(), Arg.Any<CancellationToken>());
        await exchangeListener.DidNotReceive().OnExceptionCaught(Arg.Any<Exception>());
    }

    [Fact]
    public async Task Execute_Notifies_And_Evaluates_Script_For_Exception() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Throws(new InvalidOperationException());
        var httpClient = new HttpClient(httpMessageHandler);
        var scriptEvaluator = Substitute.For<IScriptEvaluator>();
        var exchangeListener = Substitute.For<IExchangeListener>();

        var subject = new ExchangeHandler(httpClient, scriptEvaluator, Substitute.For<IServerCertificateValidationHandler>(), Substitute.For<ILoggerFactory>());

        var request = new RequestTemplateSnapshot(
            new([]),
            "Name",
            "GET",
            "https://localhost/",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "PreExchangeScript"),
            new([], "PostExchangeScript"),
            new([], "OnExceptionScript")
        );

        var result = await subject.Execute(request, new(), new(true, null, null), exchangeListener, CancellationToken.None);

        Received.InOrder(async () => {
            await scriptEvaluator.Evaluate(request.PreExchangeScript, result, CancellationToken.None);
            await exchangeListener.OnRequestCreated(Arg.Is<HttpRequestMessage>(request => request == result.Request));
            await httpMessageHandler.Received().SendAsyncCore(Arg.Is<HttpRequestMessage>(request => request == result.Request), Arg.Any<CancellationToken>());
            await exchangeListener.Received().OnRequestElapsed(Arg.Any<TimeSpan>());
            await scriptEvaluator.Evaluate(request.OnExceptionScript, result, CancellationToken.None);
            await exchangeListener.OnExceptionCaught(Arg.Is<Exception>(exception => exception == result.Exception));
        });

        await scriptEvaluator.DidNotReceive().Evaluate(request.PostExchangeScript, Arg.Any<ExchangeContext>(), Arg.Any<CancellationToken>());
        await exchangeListener.DidNotReceive().OnResponseReceived(Arg.Any<HttpResponseMessage>());
    }

    [Fact]
    public async Task Execute_Handles_Script_Execution_Exception() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Throws(new InvalidOperationException());
        var httpClient = new HttpClient(httpMessageHandler);
        var scriptEvaluator = Substitute.For<IScriptEvaluator>();
        var expectedException = new InvalidOperationException();
        scriptEvaluator.Evaluate(Arg.Is<ScriptSnapshot>(script => script.Code == "OnExceptionScript"), Arg.Any<ExchangeContext>(), CancellationToken.None).Throws(expectedException);
        var exchangeListener = Substitute.For<IExchangeListener>();

        var subject = new ExchangeHandler(httpClient, scriptEvaluator, Substitute.For<IServerCertificateValidationHandler>(), Substitute.For<ILoggerFactory>());

        var request = new RequestTemplateSnapshot(
            new([]),
            "Name",
            "GET",
            "https://localhost/",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "PreExchangeScript"),
            new([], "PostExchangeScript"),
            new([], "OnExceptionScript")
        );

        var result = await subject.Execute(request, new(), new(true, null, null), exchangeListener, CancellationToken.None);

        await exchangeListener.Received(1).OnExceptionCaught(Arg.Is<Exception>(exception => exception == result.Exception));
    }

    [Fact]
    public async Task Execute_Does_Not_Evaluate_Script_If_Not_Allowed() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(new HttpResponseMessage());
        var httpClient = new HttpClient(httpMessageHandler);
        var scriptEvaluator = Substitute.For<IScriptEvaluator>();

        var subject = new ExchangeHandler(httpClient, scriptEvaluator, Substitute.For<IServerCertificateValidationHandler>(), Substitute.For<ILoggerFactory>());

        var request = new RequestTemplateSnapshot(
            new([]),
            "Name",
            "GET",
            "https://localhost/",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "PreExchangeScript"),
            new([], "PostExchangeScript"),
            new([], "OnExceptionScript")
        );

        await subject.Execute(request, new(), new(false, null, null), CancellationToken.None);

        await scriptEvaluator.DidNotReceive().Evaluate(Arg.Any<ScriptSnapshot>(), Arg.Any<ExchangeContext>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_Does_Not_Evaluate_Exception_Script_If_Not_Allowed() {
        var httpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();
        httpMessageHandler.SendAsyncCore(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Throws(new InvalidOperationException());
        var httpClient = new HttpClient(httpMessageHandler);
        var scriptEvaluator = Substitute.For<IScriptEvaluator>();

        var subject = new ExchangeHandler(httpClient, scriptEvaluator, Substitute.For<IServerCertificateValidationHandler>(), Substitute.For<ILoggerFactory>());

        var request = new RequestTemplateSnapshot(
            new([]),
            "Name",
            "GET",
            "https://localhost/",
            [],
            ContentType.None,
            false,
            "StringContent",
            "FileContent",
            [],
            [],
            new([], "PreExchangeScript"),
            new([], "PostExchangeScript"),
            new([], "OnExceptionScript")
        );

        await subject.Execute(request, new(), new(false, null, null), CancellationToken.None);

        await scriptEvaluator.DidNotReceive().Evaluate(Arg.Any<ScriptSnapshot>(), Arg.Any<ExchangeContext>(), Arg.Any<CancellationToken>());
    }
}
