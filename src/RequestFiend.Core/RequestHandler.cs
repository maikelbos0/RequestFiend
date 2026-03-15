using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public class RequestHandler : IRequestHandler {
    private readonly HttpClient httpClient;

    public RequestHandler(HttpClient httpClient) {
        this.httpClient = httpClient;
    }

    public Task<RequestContext> Execute(RequestTemplate request, RequestTemplateCollection collection, CancellationToken cancellationToken)
        => Execute(request, collection, null, cancellationToken);

    public async Task<RequestContext> Execute(RequestTemplate request, RequestTemplateCollection collection, IRequestExchangeListener? requestExchangeListener, CancellationToken cancellationToken) {
        var context = new RequestContext();

        try {
            context.Request = request.CreateMessage(collection);
            requestExchangeListener?.OnRequestCreated(context.Request);

            context.Response = await httpClient.SendAsync(context.Request, cancellationToken);
            requestExchangeListener?.OnResponseReceived(context.Response);
        }
        catch (Exception exception) {
            context.Exception = exception;
            requestExchangeListener?.OnExceptionCaught(exception);
        }

        return context;
    }
}
