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
            EndpointAddress endpointAddress = FindCalculatorServiceAddress();

            if (endpointAddress != null)
            {
                InvokeCalculatorService(endpointAddress);
            }

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }

        static EndpointAddress FindCalculatorServiceAddress()
        {
            // Create DiscoveryClient
            DiscoveryEndpoint discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8001/DiscoveryRouter/"));
            DiscoveryClient discoveryClient = new DiscoveryClient(discoveryEndpoint);

            Console.WriteLine("Finding ICalculatorService endpoints...");
            Console.WriteLine();

            // Find ICalculatorService endpoints            
            FindCriteria findCriteria = new FindCriteria(typeof(ICalculatorService));
            findCriteria.Duration = TimeSpan.FromSeconds(10);

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
            CalculatorServiceClient client = new CalculatorServiceClient(new WSHttpBinding(), endpointAddress);            

            Console.WriteLine("Invoking CalculatorService at {0}", endpointAddress);

            double value1 = 100.00D;
            double value2 = 15.99D;

            // Call the Add service operation.
            double result = client.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }
    }
}
