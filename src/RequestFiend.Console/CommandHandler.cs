using RequestFiend.Core;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Console;

public class CommandHandler {
    private readonly IExchangeHandler exchangeHandler;

    public CommandHandler(IExchangeHandler exchangeHandler) {
        this.exchangeHandler = exchangeHandler;
    }

    public async Task ExecuteRequests(
        RequestTemplateCollection collection,
        ExchangeOptions exchangeOptions,
        Environment? environment,
        CancellationToken cancellationToken
    ) {
        foreach (var request in collection.Requests) {
            await exchangeHandler.Execute(
                request.CreateSnapshot(collection, environment),
                collection,
                exchangeOptions,
                cancellationToken
            );
        }
    }
}
