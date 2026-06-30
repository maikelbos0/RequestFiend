using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RequestFiend.Core;

public class ExchangeContext {
    public Dictionary<string, object> ExchangeData { get; } = [];
    public Dictionary<string, object> SessionData { get; }
    public Dictionary<string, string> SessionVariables { get; }
    public ILogger<ExchangeContext> Logger { get; }

    public HttpRequestMessage? Request { get; internal set; }
    public Exception? Exception { get; internal set; }
    public HttpResponseMessage? Response { get; internal set; }

    internal ExchangeContext(Dictionary<string, object> sessionData, Dictionary<string, string> sessionVariables, ILogger<ExchangeContext> logger) {
        SessionData = sessionData;
        SessionVariables = sessionVariables;
        Logger = logger;
    }
}
