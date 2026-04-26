using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RequestFiend.Core;

public interface IServerCertificateValidationHandler {
    bool IgnoreRemoteCertificateNotAvailable { get; set; }
    bool IgnoreRemoteCertificateNameMismatch { get; set; }
    bool IgnoreRemoteCertificateChainErrors { get; set; }

    bool Handle(HttpRequestMessage httpRequestMessage, X509Certificate2? x509Certificate2, X509Chain? x509Chain, SslPolicyErrors sslPolicyErrors);
}
