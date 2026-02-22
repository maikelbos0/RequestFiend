using System;
using System.Net.Http;

namespace RequestFiend.Core;

public class RequestContext {
    public HttpRequestMessage? Request { get; internal set; }
    public Exception? Exception { get; internal set; }
    public HttpResponseMessage? Response { get; internal set; }
}
