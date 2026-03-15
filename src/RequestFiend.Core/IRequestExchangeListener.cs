using System;
using System.Net.Http;

namespace RequestFiend.Core;

public interface IRequestExchangeListener {
    void OnRequestCreated(HttpRequestMessage request);
    void OnResponseReceived(HttpResponseMessage response);
    void OnExceptionCaught(Exception exception);
}
