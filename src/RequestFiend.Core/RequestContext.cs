using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RequestFiend.Core;

public class RequestContext {
    public ILogger<RequestContext> Logger { get; }
    public Dictionary<string, object> RequestData { get; } = [];
    public HttpRequestMessage? Request { get; internal set; }
    public Exception? Exception { get; internal set; }
    public HttpResponseMessage? Response { get; internal set; }

    internal RequestContext(ILogger<RequestContext> logger) {
        Logger = logger;
    }
}
