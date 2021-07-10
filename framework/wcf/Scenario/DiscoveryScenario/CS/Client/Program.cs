//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{

    class Program
    {
        public static void Main()
        {
            Console.WriteLine(" **** Client looking for ICalculatorService endpoints ****");
            EndpointAddress endpointAddress = Program.FindCalculatorServiceAddress();

            if (endpointAddress != null)
            {
                Program.InvokeCalculatorService(endpointAddress);
            }

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }

        static EndpointAddress FindCalculatorServiceAddress()
        {
            // Create a discovery client using the secure endpoint that applies and checks the compact signature
            DiscoveryClient discoveryClient = new DiscoveryClient("udpSecureDiscoveryEndpoint");

            Console.WriteLine("Finding ICalculatorService endpoints...");
            Console.WriteLine();

            FindCriteria findCriteria = new FindCriteria(typeof(ICalculatorService));
            findCriteria.Duration = TimeSpan.FromSeconds(5);

            // Find ICalculatorService endpoints    
            FindResponse findResponse = discoveryClient.Find(findCriteria);

            Console.WriteLine("Found {0} ICalculatorService endpoint(s).", findResponse.Endpoints.Count);
            Console.WriteLine();
            if (findResponse.Endpoints.Count > 0)
            {
                return findResponse.Endpoints[0].Address;
            }
            else
            {
                return null;
            }
        }

        static void InvokeCalculatorService(EndpointAddress endpointAddress)
        {
            // Create a client
            CalculatorServiceClient client = new CalculatorServiceClient();

            // Connect to the discovered service endpoint
            client.Endpoint.Address = endpointAddress;

            Console.WriteLine("Invoking CalculatorService at {0}", endpointAddress);

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

            // Close the client to close the connection and clean up resources
            client.Close();
        }
    }
}
