using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace RequestFiend.Core;

public class ExchangeHandler : IExchangeHandler {
    private readonly HttpClient httpClient;
    private readonly IScriptEvaluator scriptEvaluator;
    private readonly IServerCertificateValidationHandler serverCertificateValidationHandler;
    private readonly ILoggerFactory loggerFactory;

    public ExchangeHandler(HttpClient httpClient, IScriptEvaluator scriptEvaluator, IServerCertificateValidationHandler serverCertificateValidationHandler, ILoggerFactory loggerFactory) {
        this.httpClient = httpClient;
        this.scriptEvaluator = scriptEvaluator;
        this.serverCertificateValidationHandler = serverCertificateValidationHandler;
        this.loggerFactory = loggerFactory;
    }

    public Task<ExchangeContext> Execute(RequestTemplate request, RequestTemplateCollection collection, ExchangeOptions exchangeOptions, CancellationToken cancellationToken)
        => Execute(request, collection, exchangeOptions, null, cancellationToken);

    public async Task<ExchangeContext> Execute(
        RequestTemplate request,
        RequestTemplateCollection collection,
        ExchangeOptions exchangeOptions,
        IExchangeListener? exchangeListener,
        CancellationToken cancellationToken
    ) {
        var variableSnapshot = collection.GetVariableSnapshot(exchangeOptions.Environment);
        var context = new ExchangeContext(
            collection.GetSessionData(),
            collection.GetSessionVariables(),
            variableSnapshot.Variables,
            loggerFactory.CreateLogger<ExchangeContext>()
        );
        Timer? timer = null;
        Stopwatch? stopwatch = null;

        serverCertificateValidationHandler.Initialize(collection);

        try {
            context.Logger.LogInformation("Starting execution of request {RequestName}", request.Name);

            if (exchangeListener != null) {
                await exchangeListener.OnVariablesCompiled(variableSnapshot.Variables);
            }

            context.Request = request.CreateMessage(collection, variableSnapshot);

            if (exchangeOptions.AllowScriptEvaluation) {
                await scriptEvaluator.Evaluate(request.PreExchangeScript, context, cancellationToken);
            }

            if (exchangeListener != null) {
                await exchangeListener.OnRequestCreated(context.Request);
            }

            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (exchangeOptions.RequestTimeoutInSeconds.HasValue) {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(exchangeOptions.RequestTimeoutInSeconds.Value));
            }

            if (exchangeListener != null) {
                stopwatch = new Stopwatch();
                timer = new Timer(42);
                timer.Elapsed += (_, _) => exchangeListener.OnRequestElapsed(stopwatch.Elapsed);
                timer.Start();
                stopwatch.Start();
            }

            context.Response = await httpClient.SendAsync(context.Request, cancellationTokenSource.Token);

            await CompleteRequestElapsed();

            if (exchangeOptions.AllowScriptEvaluation) {
                await scriptEvaluator.Evaluate(request.PostExchangeScript, context, cancellationToken);
            }

            if (exchangeListener != null) {
                await exchangeListener.OnResponseReceived(context.Response);
            }
            context.Logger.LogInformation("Finished execution of request {RequestName}", request.Name);
        }
        catch (Exception exception) {
            await CompleteRequestElapsed();
            context.Logger.LogError(exception, "Exception occurred during execution of request {RequestName}", request.Name);

            context.Exception = exception;

            if (exchangeOptions.AllowScriptEvaluation) {
                try {
                    await scriptEvaluator.Evaluate(request.OnExceptionScript, context, cancellationToken);
                }
                catch (Exception scriptException) {
                    context.Exception = scriptException;
                }
            }

            if (exchangeListener != null) {
                await exchangeListener.OnExceptionCaught(context.Exception);
            }
        }

        return context;

        async Task CompleteRequestElapsed() {
            stopwatch?.Stop();
            timer?.Stop();
            timer?.Dispose();

            if (exchangeListener != null && stopwatch != null) {
                await exchangeListener.OnRequestElapsed(stopwatch.Elapsed);
            }
        }
    }
}
