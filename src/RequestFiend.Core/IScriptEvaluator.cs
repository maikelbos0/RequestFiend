using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface IScriptEvaluator {
    [Obsolete]
    Task Evaluate(Script script, ExchangeContext context, CancellationToken cancellationToken);
    Task Evaluate(ScriptSnapshot script, ExchangeContext context, CancellationToken cancellationToken);
}