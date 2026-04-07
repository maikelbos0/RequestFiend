using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface IRequestHandler {
    Task<RequestContext> Execute(RequestTemplate request, RequestTemplateCollection collection, RequestExchangeOptions requestExchangeOptions, CancellationToken cancellationToken);
    Task<RequestContext> Execute(
        RequestTemplate request,
        RequestTemplateCollection collection,
        RequestExchangeOptions requestExchangeOptions,
        IRequestExchangeListener? requestExchangeListener,
        CancellationToken cancellationToken
    );
}