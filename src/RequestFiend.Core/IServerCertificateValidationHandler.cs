using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RequestFiend.Core;

public interface IServerCertificateValidationHandler {
    void Initialize(RequestTemplateCollection collection);

    bool Handle(object sender, X509Certificate? x509Certificate2, X509Chain? x509Chain, SslPolicyErrors sslPolicyErrors);
}
