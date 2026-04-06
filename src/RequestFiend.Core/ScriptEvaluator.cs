using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public class ScriptEvaluator : IScriptEvaluator {
    private static readonly ScriptOptions scriptOptions = ScriptOptions.Default.WithReferences([
        typeof(HttpClient).Assembly,
        typeof(ILogger).Assembly
    ]).WithImports([
        $"{nameof(Microsoft)}",
        $"{nameof(Microsoft)}.{nameof(Microsoft.Extensions)}",
        $"{nameof(Microsoft)}.{nameof(Microsoft.Extensions)}.{nameof(Microsoft.Extensions.Logging)}",
        $"{nameof(Microsoft)}.{nameof(Microsoft.Extensions)}.{nameof(Microsoft.Extensions.Logging)}.{nameof(Microsoft.Extensions.Logging.Abstractions)}",
        $"{nameof(System)}",
        $"{nameof(System)}.{nameof(System.Collections)}",
        $"{nameof(System)}.{nameof(System.Collections)}.{nameof(System.Collections.Generic)}",
        $"{nameof(System)}.{nameof(System.Net)}",
        $"{nameof(System)}.{nameof(System.Net)}.{nameof(System.Net.Http)}",
    ]);


    public Task Evaluate(string script, RequestContext context, CancellationToken cancellationToken)
        => CSharpScript.EvaluateAsync(script, scriptOptions, context, cancellationToken: cancellationToken);
}
