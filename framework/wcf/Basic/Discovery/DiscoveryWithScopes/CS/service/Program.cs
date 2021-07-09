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
            Uri baseAddress = new Uri("http://localhost:8000/" + Guid.NewGuid().ToString());

            ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService), baseAddress);

            try
            {
                // Add an endpoint to the service
                ServiceEndpoint discoverableCalculatorEndpoint = serviceHost.AddServiceEndpoint(
                    typeof(ICalculatorService), 
                    new WSHttpBinding(), 
                    "/DiscoverableEndpoint");

                // Add a Scope to the endpoint
                EndpointDiscoveryBehavior discoverableEndpointBehavior = new EndpointDiscoveryBehavior();
                discoverableEndpointBehavior.Scopes.Add(new Uri("ldap:///ou=engineering,o=exampleorg,c=us"));
                discoverableCalculatorEndpoint.Behaviors.Add(discoverableEndpointBehavior);

                // Add an endpoint to the service
                ServiceEndpoint nonDiscoverableCalculatorEndpoint = serviceHost.AddServiceEndpoint
                    (typeof(ICalculatorService), 
                    new WSHttpBinding(), 
                    "/NonDiscoverableEndpoint");

                // Disable discoverability of the endpoint
                EndpointDiscoveryBehavior nonDiscoverableEndpointBehavior = new EndpointDiscoveryBehavior();
                nonDiscoverableEndpointBehavior.Scopes.Add(new Uri("ldap:///ou=engineering,o=exampleorg,c=us"));
                nonDiscoverableEndpointBehavior.Enabled = false;
                nonDiscoverableCalculatorEndpoint.Behaviors.Add(nonDiscoverableEndpointBehavior);

                // Make the service discoverable over UDP multicast
                serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());

                serviceHost.Open();

                Console.WriteLine("Calculator Service started at {0}", baseAddress);
                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to terminate the service.");
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }

            if (serviceHost.State != CommunicationState.Closed)
            {
                Console.WriteLine("Aborting the service...");
                serviceHost.Abort();
            }
        }
    }
}
