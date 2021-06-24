//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{
    class Program
    {
        public static void Main()
        {
            try
            {
                DynamicEndpoint dynamicEndpoint = new DynamicEndpoint(ContractDescription.GetContract(typeof(ICalculatorService)), new NetTcpBinding());

                Uri redmondScope = new Uri("net.tcp://Microsoft.Samples.Discovery/RedmondLocation");
                Uri seattleScope = new Uri("net.tcp://Microsoft.Samples.Discovery/SeattleLocation");
                Uri portlandScope = new Uri("net.tcp://Microsoft.Samples.Discovery/PortlandLocation");

                dynamicEndpoint.FindCriteria.Scopes.Add(redmondScope);
                dynamicEndpoint.FindCriteria.Scopes.Add(seattleScope);
                dynamicEndpoint.FindCriteria.Scopes.Add(portlandScope);
                
                // Specify the custom ScopeMatchBy
                dynamicEndpoint.FindCriteria.ScopeMatchBy = new Uri("net.tcp://Microsoft.Samples.Discovery/ORExactMatch");

                CalculatorServiceClient client = new CalculatorServiceClient(dynamicEndpoint);

                Console.WriteLine("Discovering CalculatorService.");
                Console.WriteLine("Looking for a Calculator Service that matches either of the scopes:");
                Console.WriteLine(" " + redmondScope);
                Console.WriteLine(" " + seattleScope);
                Console.WriteLine(" " + portlandScope);
                Console.WriteLine();

                double value1 = 1023;
                double value2 = 1534;
                double value3 = 2342;

                // Call the Add service operation.
                double result = client.Add(value1, value2);
                Console.WriteLine("Adding({0}, {1}) = {2}", value1, value2, result);

                // Call the Subtract service operation.
                result = client.Subtract(value3, value2);
                Console.WriteLine("Subtracting ({0}, {1}) = {2}", value3, value2, result);

                //Closing the client gracefully closes the connection and cleans up resources
                client.Close();
            }
            catch (EndpointNotFoundException)
            {
                Console.WriteLine("Unable to connect to the calculator service because a valid endpoint was not found.");
            }

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }
    }
}
