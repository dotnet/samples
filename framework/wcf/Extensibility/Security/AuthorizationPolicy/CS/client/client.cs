//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.AuthorizationPolicy
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.
    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client with Username endpoint configuration
            CalculatorClient client1 = new CalculatorClient("Username");

            client1.ClientCredentials.UserName.UserName = "test1";
            client1.ClientCredentials.UserName.Password = "1tset";

            try
            {
                // Call the Add service operation.
                double value1 = 100.00D;
                double value2 = 15.99D;
                double result = client1.Add(value1, value2);
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

                // Call the Subtract service operation.
                value1 = 145.00D;
                value2 = 76.54D;
                result = client1.Subtract(value1, value2);
                Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

                // Call the Multiply service operation.
                value1 = 9.00D;
                value2 = 81.25D;
                result = client1.Multiply(value1, value2);
                Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

                // Call the Divide service operation.
                value1 = 22.00D;
                value2 = 7.00D;
                result = client1.Divide(value1, value2);
                Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
            }
            catch (Exception e)
            {
                Console.WriteLine("Call failed : {0}", e.Message);
            }

            client1.Close();

            // Create a client with Certificate endpoint configuration
            CalculatorClient client2 = new CalculatorClient("Certificate");

            client2.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySubjectName, "test1");

            try
            {
                // Call the Add service operation.
                double value1 = 100.00D;
                double value2 = 15.99D;
                double result = client2.Add(value1, value2);
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

                // Call the Subtract service operation.
                value1 = 145.00D;
                value2 = 76.54D;
                result = client2.Subtract(value1, value2);
                Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

                // Call the Multiply service operation.
                value1 = 9.00D;
                value2 = 81.25D;
                result = client2.Multiply(value1, value2);
                Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

                // Call the Divide service operation.
                value1 = 22.00D;
                value2 = 7.00D;
                result = client2.Divide(value1, value2);
                Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
            }
            catch (Exception e)
            {
                Console.WriteLine("Call failed : {0}", e.Message);
            }

            client2.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}

