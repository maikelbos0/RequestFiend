using RequestFiend.Core;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Console;

public interface ICommandHandler {
    Task ExecuteRequests(RequestTemplateCollection collection, ExchangeOptions exchangeOptions, Environment? environment, CancellationToken cancellationToken);
}