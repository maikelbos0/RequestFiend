using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public class RequestHandler {
    private readonly HttpClient httpClient;

    public RequestHandler(HttpClient httpClient) {
        this.httpClient = httpClient;
    }

    public async Task<RequestContext> Execute(RequestTemplate request, RequestTemplateCollection collection, CancellationToken cancellationToken) {
        var context = new RequestContext();

        try {
            context.Request = request.CreateMessage(collection);
            context.Response = await httpClient.SendAsync(context.Request, cancellationToken);
        }
        catch (Exception exception) {
            context.Exception = exception;
        }

        return context;
    }
}
