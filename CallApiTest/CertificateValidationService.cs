using System.Security.Cryptography.X509Certificates;

namespace CallApiTest
{
    public class CertificateValidationService
    {
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            var cert = new X509Certificate2(@"C:\Users\admin\RiderProjects\cert\client.pfx", "1234");

            if (clientCertificate.Thumbprint == cert.Thumbprint)
            {
                return true;
            }

            return false;

        }     
    }
}
