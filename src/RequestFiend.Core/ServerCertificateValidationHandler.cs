using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RequestFiend.Core;

public class ServerCertificateValidationHandler : IServerCertificateValidationHandler {
    public bool IgnoreRemoteCertificateNotAvailable { get; set; }
    public bool IgnoreRemoteCertificateNameMismatch { get; set; }
    public bool IgnoreRemoteCertificateChainErrors { get; set; }

    public bool Handle(HttpRequestMessage httpRequestMessage, X509Certificate2? x509Certificate2, X509Chain? x509Chain, SslPolicyErrors sslPolicyErrors) {
        if (!IgnoreRemoteCertificateNotAvailable && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable)) {
            return false;
        }

        if (!IgnoreRemoteCertificateNameMismatch && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch)) {
            return false;
        }

        if (!IgnoreRemoteCertificateChainErrors && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors)) {
            return false;
        }

        return true;
    }
}
