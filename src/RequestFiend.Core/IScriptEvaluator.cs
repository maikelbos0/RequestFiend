using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface IScriptEvaluator {
    Task Evaluate(string script, RequestContext context, CancellationToken cancellationToken);
}