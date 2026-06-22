using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;

namespace RequestFiend.Core;

public class RequestContext {
    public Dictionary<string, object> RequestData { get; } = [];
    public Dictionary<string, object> SessionData { get; }
    public Dictionary<string, string> SessionVariables { get; }
    public ImmutableDictionary<string, string> Variables { get; }
    public ILogger<RequestContext> Logger { get; }

    public HttpRequestMessage? Request { get; internal set; }
    public Exception? Exception { get; internal set; }
    public HttpResponseMessage? Response { get; internal set; }

    internal RequestContext(Dictionary<string, object> sessionData, Dictionary<string, string> sessionVariables, ImmutableDictionary<string, string> variables, ILogger<RequestContext> logger) {
        SessionData = sessionData;
        SessionVariables = sessionVariables;
        Variables = variables;
        Logger = logger;
    }
}
