using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public class RequestHandler : IRequestHandler {
    private readonly HttpClient httpClient;
    private readonly IScriptEvaluator scriptEvaluator;
    private readonly IServerCertificateValidationHandler serverCertificateValidationHandler;
    private readonly ILoggerFactory loggerFactory;

    public RequestHandler(HttpClient httpClient, IScriptEvaluator scriptEvaluator, IServerCertificateValidationHandler serverCertificateValidationHandler, ILoggerFactory loggerFactory) {
        this.httpClient = httpClient;
        this.scriptEvaluator = scriptEvaluator;
        this.serverCertificateValidationHandler = serverCertificateValidationHandler;
        this.loggerFactory = loggerFactory;
    }

    public Task<RequestContext> Execute(RequestTemplate request, RequestTemplateCollection collection, RequestExchangeOptions requestExchangeOptions, CancellationToken cancellationToken)
        => Execute(request, collection, requestExchangeOptions, null, cancellationToken);

    public async Task<RequestContext> Execute(
        RequestTemplate request,
        RequestTemplateCollection collection,
        RequestExchangeOptions requestExchangeOptions,
        IRequestExchangeListener? requestExchangeListener,
        CancellationToken cancellationToken
    ) {
        var context = new RequestContext(collection.GetSessionData(), collection.GetSessionVariables(), loggerFactory.CreateLogger<RequestContext>());

        serverCertificateValidationHandler.Initialize(collection);

        try {
            context.Request = request.CreateMessage(collection);

            if (requestExchangeOptions.AllowScriptEvaluation) {
                await scriptEvaluator.Evaluate(request.PreExchangeScript, context, cancellationToken);
            }

            if (requestExchangeListener != null) {
                await requestExchangeListener.OnRequestCreated(context.Request);
            }

            context.Response = await httpClient.SendAsync(context.Request, cancellationToken);

            if (requestExchangeOptions.AllowScriptEvaluation) {
                await scriptEvaluator.Evaluate(request.PostExchangeScript, context, cancellationToken);
            }

            if (requestExchangeListener != null) {
                await requestExchangeListener.OnResponseReceived(context.Response);
            }
        }
        catch (Exception exception) {
            context.Exception = exception;

            if (requestExchangeOptions.AllowScriptEvaluation) {
                try {
                    await scriptEvaluator.Evaluate(request.OnExceptionScript, context, cancellationToken);
                }
                catch (Exception scriptException) {
                    context.Exception = scriptException;
                }
            }

            if (requestExchangeListener != null) {
                await requestExchangeListener.OnExceptionCaught(context.Exception);
            }
        }

        return context;
    }
}
