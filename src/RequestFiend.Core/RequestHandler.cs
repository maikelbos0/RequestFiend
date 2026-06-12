using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

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
        Timer? timer = null;
        Stopwatch? stopwatch = null;

        serverCertificateValidationHandler.Initialize(collection);

        try {
            context.Logger.LogInformation("Starting execution of request {RequestName}", request.Name);
            context.Request = request.CreateMessage(collection);

            if (requestExchangeOptions.AllowScriptEvaluation) {
                await scriptEvaluator.Evaluate(request.PreExchangeScript, context, cancellationToken);
            }

            if (requestExchangeListener != null) {
                await requestExchangeListener.OnRequestCreated(context.Request);
            }

            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (requestExchangeOptions.RequestTimeoutInSeconds.HasValue) {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(requestExchangeOptions.RequestTimeoutInSeconds.Value));
            }

            if (requestExchangeListener != null) {
                stopwatch = new Stopwatch();
                timer = new Timer(42);
                timer.Elapsed += (_, _) => requestExchangeListener.OnRequestElapsed(stopwatch.Elapsed);
                timer.Start();
                stopwatch.Start();
            }

            context.Response = await httpClient.SendAsync(context.Request, cancellationTokenSource.Token);

            await CompleteRequestElapsed();

            if (requestExchangeOptions.AllowScriptEvaluation) {
                await scriptEvaluator.Evaluate(request.PostExchangeScript, context, cancellationToken);
            }

            if (requestExchangeListener != null) {
                await requestExchangeListener.OnResponseReceived(context.Response);
            }
            context.Logger.LogInformation("Finished execution of request {RequestName}", request.Name);
        }
        catch (Exception exception) {
            await CompleteRequestElapsed();
            context.Logger.LogError(exception, "Exception occurred during execution of request {RequestName}", request.Name);

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

        async Task CompleteRequestElapsed() {
            stopwatch?.Stop();
            timer?.Stop();
            timer?.Dispose();

            if (requestExchangeListener != null && stopwatch != null) {
                await requestExchangeListener.OnRequestElapsed(stopwatch.Elapsed);
            }
        }
    }
}
