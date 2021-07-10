
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.Samples.ClientValidation
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Get all serviceEndpoints from the MEX endpoint of the service.
            ServiceEndpointCollection serviceEndpoints = MetadataResolver.Resolve(typeof(ICalculator), new EndpointAddress("http://localhost:8000/ServiceModelSamples/service/mex"));

            foreach (ServiceEndpoint endpoint in serviceEndpoints)
            {
                try
                {
                    Console.WriteLine("Creating a client with connecting to endpoint address {0}.", endpoint.Address.ToString());

                    CalculatorClient client = new CalculatorClient(endpoint.Binding, endpoint.Address);

                    // Add the InternetClientValidatorBehavior to the endpoint.
                    // The behaviors are evaluated when the client creates a ChannelFactory.
                    client.Endpoint.Behaviors.Add(new InternetClientValidatorBehavior());

                    client.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySubjectName, "client.com");
                    client.ClientCredentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.CurrentUser, StoreName.TrustedPeople, X509FindType.FindBySubjectName, "localhost");
                    client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;

                    // Call operations
                    // All invalid enpoints will fail when this is called.
                    DoCalculations(client);

                    client.Close();
                }
                catch(InvalidOperationException ex)
                {
                    // This exception is thrown in InternetClientValidatorBehavior.
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void DoCalculations(ICalculator client)
        {
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
        }
    }
}

