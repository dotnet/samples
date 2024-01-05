//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace Microsoft.Samples.TransportSecurity
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // WARNING: This code is only needed for test certificates such as those created by makecert. It is 
            // not recommended for production code.
            // Create a client
            CalculatorClient client = new CalculatorClient();
            client.ClientCredentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
            client.ClientCredentials.ServiceCertificate.SslCertificateAuthentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            MyX509CertificateValidator myX509CertificateValidator = new MyX509CertificateValidator("CN=ServiceModelSamples-HTTPS-Server");
            client.ClientCredentials.ServiceCertificate.SslCertificateAuthentication.CustomCertificateValidator = myX509CertificateValidator;

            // Call the Add service operation.
            double value1 = 100.00D;
            double value2 = 15.99D;
            double result = client.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation.
            value1 = 145.00D;
            value2 = 76.54D;
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            value1 = 9.00D;
            value2 = 81.25D;
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            value1 = 22.00D;
            value2 = 7.00D;
            result = client.Divide(value1, value2);
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }

    // WARNING: This code is only needed for test certificates such as those created by makecert. It is 
    // not recommended for production code.
    public class MyX509CertificateValidator : X509CertificateValidator
    {
        private string _allowedIssuerName;

        public MyX509CertificateValidator(string allowedIssuerName)
        {
            if (string.IsNullOrEmpty(allowedIssuerName))
            {
                throw new ArgumentNullException("allowedIssuerName", "[MyX509CertificateValidator] The string parameter allowedIssuerName was null or empty.");
            }

            _allowedIssuerName = allowedIssuerName;
        }

        public override void Validate(X509Certificate2 certificate)
        {
            // Check that there is a certificate.
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate", "[MyX509CertificateValidator] The X509Certificate2 parameter certificate was null.");
            }

            // Check that the certificate issuer matches the configured issuer.
            if (!certificate.Subject.Contains(_allowedIssuerName))
            {
                throw new SecurityTokenValidationException
                  (string.Format("Certificate was not issued by a trusted issuer. Expected: {0}, Actual: {1}", _allowedIssuerName, certificate.IssuerName.Name));
            }
        }
    }
}



