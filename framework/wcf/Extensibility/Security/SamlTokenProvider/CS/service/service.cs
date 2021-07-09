//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.IdentityModel.Claims;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace Microsoft.Samples.SamlTokenProvider
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.SamlTokenProvider")]
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

    // Service class which implements the service contract.
    // Added code to write output to the console window
    public class CalculatorService : ICalculator
    {
        static void DisplayClaims()
        {
            Console.WriteLine("******");

            foreach (ClaimSet cs in OperationContext.Current.ServiceSecurityContext.AuthorizationContext.ClaimSets)
            {
                Console.WriteLine(cs);                
            }

            Console.WriteLine("******");
        }

        public double Add(double n1, double n2)
        {
            DisplayClaims();
            double result = n1 + n2;
            Console.WriteLine("Received Add({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Subtract(double n1, double n2)
        {
            DisplayClaims();
            double result = n1 - n2;
            Console.WriteLine("Received Subtract({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Multiply(double n1, double n2)
        {
            DisplayClaims();
            double result = n1 * n2;
            Console.WriteLine("Received Multiply({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Divide(double n1, double n2)
        {
            DisplayClaims();
            double result = n1 / n2;
            Console.WriteLine("Received Divide({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }


        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type and provide the base address.
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService)))
            {
                // Set validation mode for issuer certificates to allow certificates in the trusted people store
                serviceHost.Credentials.IssuedTokenAuthentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;
                // Open the ServiceHost to create listeners and start listening for messages.
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
}

