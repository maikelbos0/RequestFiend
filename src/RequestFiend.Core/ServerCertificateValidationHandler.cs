using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace RequestFiend.Core;

public class ServerCertificateValidationHandler : IServerCertificateValidationHandler {
    public AsyncLocal<bool> IgnoreRemoteCertificateNotAvailable { get; set; } = new();
    public AsyncLocal<bool> IgnoreRemoteCertificateNameMismatch { get; set; } = new();
    public AsyncLocal<bool> IgnoreRemoteCertificateChainErrors { get; set; } = new();

    public void Initialize(RequestTemplateCollection collection) {
        IgnoreRemoteCertificateNotAvailable.Value = collection.IgnoreRemoteCertificateNotAvailable;
        IgnoreRemoteCertificateNameMismatch.Value = collection.IgnoreRemoteCertificateNameMismatch;
        IgnoreRemoteCertificateChainErrors.Value = collection.IgnoreRemoteCertificateChainErrors;
    }

    public bool Handle(HttpRequestMessage httpRequestMessage, X509Certificate2? x509Certificate2, X509Chain? x509Chain, SslPolicyErrors sslPolicyErrors) {
        if (!IgnoreRemoteCertificateNotAvailable.Value && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable)) {
            return false;
        }

        if (!IgnoreRemoteCertificateNameMismatch.Value && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch)) {
            return false;
        }

        if (!IgnoreRemoteCertificateChainErrors.Value && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors)) {
            return false;
        }

        return true;
    }
}
