using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequester
{
    public static class NetHelper
    {
        public static void AcceptInvalidSslCertificate()
        {
            // https://stackoverflow.com/questions/1301127/how-to-ignore-a-certificate-error-with-c-sharp-2-0-webclient-without-the-certi

            // Change SSL checks so that all checks pass
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(
                    delegate
                    { return true; }
                );
        }
    }
}
