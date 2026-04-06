using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public class ScriptEvaluator : IScriptEvaluator {
    private static readonly ScriptOptions scriptOptions = ScriptOptions.Default.WithReferences([
        typeof(HttpClient).Assembly
    ]);

    public Task Evaluate(string script, RequestContext context, CancellationToken cancellationToken)
        => CSharpScript.EvaluateAsync(script, scriptOptions, context, cancellationToken: cancellationToken);
}
