using System.Net.Security;
using Xunit;

namespace RequestFiend.Core.Tests;

public class ServerCertificateValidationHandlerTests {
    [Theory]
    [InlineData(SslPolicyErrors.None, false, false, false, true)]
    [InlineData(SslPolicyErrors.RemoteCertificateNotAvailable, false, false, false, false)]
    [InlineData(SslPolicyErrors.RemoteCertificateNotAvailable, true, false, false, true)]
    [InlineData(SslPolicyErrors.RemoteCertificateNameMismatch, false, false, false, false)]
    [InlineData(SslPolicyErrors.RemoteCertificateNameMismatch, false, true, false, true)]
    [InlineData(SslPolicyErrors.RemoteCertificateChainErrors, false, false, false, false)]
    [InlineData(SslPolicyErrors.RemoteCertificateChainErrors, false, false, true, true)]
    [InlineData(SslPolicyErrors.RemoteCertificateNotAvailable | SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors, true, false, false, false)]
    [InlineData(SslPolicyErrors.RemoteCertificateNotAvailable | SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors, false, true, false, false)]
    [InlineData(SslPolicyErrors.RemoteCertificateNotAvailable | SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors, false, false, true, false)]
    [InlineData(SslPolicyErrors.RemoteCertificateNotAvailable | SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors, true, true, true, true)]
    public void Handle(SslPolicyErrors sslPolicyErrors, bool ignoreRemoteCertificateNotAvailable, bool ignoreRemoteCertificateNameMismatch, bool ignoreRemoteCertificateChainErrors, bool expectedResult) {
        var subject = new ServerCertificateValidationHandler() {
            IgnoreRemoteCertificateNotAvailable = ignoreRemoteCertificateNotAvailable,
            IgnoreRemoteCertificateNameMismatch = ignoreRemoteCertificateNameMismatch,
            IgnoreRemoteCertificateChainErrors = ignoreRemoteCertificateChainErrors
        };

        Assert.Equal(expectedResult, subject.Handle(new(), null, null, sslPolicyErrors));
    }
}
