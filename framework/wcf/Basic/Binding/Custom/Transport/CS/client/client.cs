
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client to default (http) endpoint configuration 
            CalculatorClient client = new CalculatorClient("default");
            Console.WriteLine("Communicate with http endpoint.");
            // call operations
            DoCalculations(client);
            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            // Create a client to tcp endpoint configuration
            client = new CalculatorClient("tcp");
            Console.WriteLine("Communicate with tcp endpoint.");
            // call operations
            DoCalculations(client);
            client.Close();

            // Create a client to named pipe endpoint configuration
            client = new CalculatorClient("namedpipe");
            Console.WriteLine("Communicate with named pipe endpoint.");
            // call operations
            DoCalculations(client);
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void DoCalculations(CalculatorClient client)
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
