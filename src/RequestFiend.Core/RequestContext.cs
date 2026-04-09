using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RequestFiend.Core;

public class RequestContext {
    public Dictionary<string, object> RequestData { get; } = [];
    public Dictionary<string, object> SessionData { get; internal set; }
    public Dictionary<string, string> SessionVariables { get; internal set; }
    public HttpRequestMessage? Request { get; internal set; }
    public Exception? Exception { get; internal set; }
    public HttpResponseMessage? Response { get; internal set; }
    public ILogger<RequestContext> Logger { get; }

    internal RequestContext(Dictionary<string, object> sessionData, Dictionary<string, string> sessionVariables, ILogger<RequestContext> logger) {
        SessionData = sessionData;
        SessionVariables = sessionVariables;
        Logger = logger;
    }
}
