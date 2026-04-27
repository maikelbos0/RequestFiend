using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace RequestFiend.Core;

public class ServerCertificateValidationHandler : IServerCertificateValidationHandler {
    private readonly AsyncLocal<bool> ignoreRemoteCertificateNotAvailable = new();
    private readonly AsyncLocal<bool> ignoreRemoteCertificateNameMismatch = new();
    private readonly AsyncLocal<bool> ignoreRemoteCertificateChainErrors = new();

    public void Initialize(RequestTemplateCollection collection) {
        ignoreRemoteCertificateNotAvailable.Value = collection.IgnoreRemoteCertificateNotAvailable;
        ignoreRemoteCertificateNameMismatch.Value = collection.IgnoreRemoteCertificateNameMismatch;
        ignoreRemoteCertificateChainErrors.Value = collection.IgnoreRemoteCertificateChainErrors;
    }

    public bool Handle(object sender, X509Certificate? x509Certificate2, X509Chain? x509Chain, SslPolicyErrors sslPolicyErrors) {
        if (!ignoreRemoteCertificateNotAvailable.Value && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable)) {
            return false;
        }

        if (!ignoreRemoteCertificateNameMismatch.Value && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch)) {
            return false;
        }

        if (!ignoreRemoteCertificateChainErrors.Value && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors)) {
            return false;
        }

        return true;
    }
}
