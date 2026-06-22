using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface IExchangeHandler {
    Task<ExchangeContext> Execute(RequestTemplate request, RequestTemplateCollection collection, ExchangeOptions exchangeOptions, CancellationToken cancellationToken);
    Task<ExchangeContext> Execute(
        RequestTemplate request,
        RequestTemplateCollection collection,
        ExchangeOptions exchangeOptions,
        IExchangeListener? exchangeListener,
        CancellationToken cancellationToken
    );
}