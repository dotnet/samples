
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;


namespace Microsoft.Samples.X509CertificateValidator
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.
    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client with Certificate endpoint configuration
            CalculatorClient client = new CalculatorClient("Certificate");

            try
            {
                client.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySubjectName, "alice");

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
                client.Close();
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("Call timed out : {0}", e.Message);
                client.Abort();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("Call failed : {0}", e.Message);
                client.Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine("Call failed : {0}", e.Message);
                client.Abort();
            }

            

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}

