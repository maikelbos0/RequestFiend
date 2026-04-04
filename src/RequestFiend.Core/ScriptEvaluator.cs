using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public class ScriptEvaluator : IScriptEvaluator {
    public Task Evaluate(string script, RequestContext context, CancellationToken cancellationToken)
        => CSharpScript.EvaluateAsync(script, globals: context, cancellationToken: cancellationToken);
}
