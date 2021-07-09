//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Discovery
{
    class Client
    {
        public static void Main()
        {
            try
            {
                InvokeCalculatorService();
            }
            catch (EndpointNotFoundException)
            {
                Console.WriteLine("Client was unable find an endpoint to connect to.");   
            }

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }

        static void InvokeCalculatorService()
        {
            // Create a client
            CalculatorServiceClient client = new CalculatorServiceClient("calculatorEndpoint");

            Console.WriteLine("Invoking CalculatorService");

            double value1 = 100.00D;
            double value2 = 15.99D;

            // Call the Add service operation.
            double result = client.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation.
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            result = client.Divide(value1, value2);
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
            Console.WriteLine();

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }
    }
}

