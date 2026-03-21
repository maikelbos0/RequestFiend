using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestFiend.Core;

public interface IRequestExchangeListener {
    Task OnRequestCreated(HttpRequestMessage request);
    Task OnResponseReceived(HttpResponseMessage response);
    Task OnExceptionCaught(Exception exception);
}
