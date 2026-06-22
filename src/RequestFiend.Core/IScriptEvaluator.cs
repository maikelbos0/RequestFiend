using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface IScriptEvaluator {
    Task Evaluate(Script script, ExchangeContext context, CancellationToken cancellationToken);
}