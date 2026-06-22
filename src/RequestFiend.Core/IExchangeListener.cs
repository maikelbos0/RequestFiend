using System;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface IExchangeListener {
    Task OnVariablesCompiled(ImmutableDictionary<string, string> variables);
    Task OnRequestCreated(HttpRequestMessage request);
    Task OnResponseReceived(HttpResponseMessage response);
    Task OnExceptionCaught(Exception exception);
    Task OnRequestElapsed(TimeSpan requestElapsed);
}
