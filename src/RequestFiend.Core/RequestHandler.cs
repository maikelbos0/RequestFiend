using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public class RequestHandler : IRequestHandler {
    private readonly HttpClient httpClient;
    private readonly IScriptEvaluator scriptEvaluator;

    public RequestHandler(HttpClient httpClient, IScriptEvaluator scriptEvaluator) {
        this.httpClient = httpClient;
        this.scriptEvaluator = scriptEvaluator;
    }

    public Task<RequestContext> Execute(RequestTemplate request, RequestTemplateCollection collection, CancellationToken cancellationToken)
        => Execute(request, collection, null, cancellationToken);

    public async Task<RequestContext> Execute(RequestTemplate request, RequestTemplateCollection collection, IRequestExchangeListener? requestExchangeListener, CancellationToken cancellationToken) {
        var context = new RequestContext();

        try {
            context.Request = request.CreateMessage(collection);
            await scriptEvaluator.Evaluate(request.PreExchangeScript, context, cancellationToken);
            if (requestExchangeListener != null) {
                await requestExchangeListener.OnRequestCreated(context.Request);
            }

            context.Response = await httpClient.SendAsync(context.Request, cancellationToken);
            await scriptEvaluator.Evaluate(request.PostExchangeScript, context, cancellationToken);
            if (requestExchangeListener != null) {
                await requestExchangeListener.OnResponseReceived(context.Response);
            }
        }
        catch (Exception exception) {
            context.Exception = exception;
            await scriptEvaluator.Evaluate(request.OnExceptionScript, context, cancellationToken);
            if (requestExchangeListener != null) {
                await requestExchangeListener.OnExceptionCaught(exception);
            }
        }

        return context;
    }
}
