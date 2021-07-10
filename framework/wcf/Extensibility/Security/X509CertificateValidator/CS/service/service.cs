
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;

namespace Microsoft.Samples.X509CertificateValidator
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.X509CertificateValidator")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

    // Service class that implements the service contract.
    // Added code to write output to the console window
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]	
    public class CalculatorService : ICalculator
    {
        public double Add(double n1, double n2)
        {
            double result = n1 + n2;
            Console.WriteLine("Received Add({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
			return result;
        }


        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;
            Console.WriteLine("Received Subtract({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }


        public double Multiply(double n1, double n2)
        {
            double result = n1 * n2;
            Console.WriteLine("Received Multiply({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

		
        public double Divide(double n1, double n2)
        {
            double result = n1 / n2;
            Console.WriteLine("Received Divide({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }        
        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService)))
            {
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
                serviceHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new CustomX509CertificateValidator();

                serviceHost.Open();
				
                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
		Console.WriteLine("The service is running in the following account: {0}", WindowsIdentity.GetCurrent().Name);
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

            }
        }
    }

    public class CustomX509CertificateValidator : System.IdentityModel.Selectors.X509CertificateValidator
    {
        // This Validation function accepts any X.509 Certificate that is self-issued. As anyone can construct such
        // a certificate this custom validator is less secure than the default behavior provided by the
        // ChainTrust X509CertificateValidationMode. The security implications of this should be carefully 
        // considered before using this validation logic in production code. 
        public override void Validate(X509Certificate2 certificate)
        {
            // Check that we have been passed a certificate
            if (certificate == null)
                throw new ArgumentNullException("certificate");

            // Only accept self-issued certificates
            if (certificate.Subject != certificate.Issuer)
                throw new SecurityTokenException("Certificate is not self-issued");
        }
    }
}

