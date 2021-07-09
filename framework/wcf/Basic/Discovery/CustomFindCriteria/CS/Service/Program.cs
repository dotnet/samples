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
            Uri baseAddress = new Uri("net.tcp://localhost:8000/CalculatorSvc/");
            Uri redmondScope = new Uri("net.tcp://Microsoft.Samples.Discovery/RedmondLocation");

            ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService), baseAddress);

            try
            {                
                ServiceEndpoint tcpEndpoint = serviceHost.AddServiceEndpoint(typeof(ICalculatorService), new NetTcpBinding(), "TCPEndpoint");

                // Add a scope
                EndpointDiscoveryBehavior tcpEndpointBehavior = new EndpointDiscoveryBehavior();
                tcpEndpointBehavior.Scopes.Add(redmondScope);
                tcpEndpoint.Behaviors.Add(tcpEndpointBehavior);                

                // Make the service discoverable
                serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());

                // Listen for discovery messages over UDP multicast
                serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());

                // Hook up a custom discovery service implementation through extensions
                serviceHost.Extensions.Add(new CustomDiscoveryExtension());

                serviceHost.Open();

                Console.WriteLine("Calculator Service started at: \n {0}", baseAddress);
                Console.WriteLine("Endpoint is decorated with the following scope: \n {0}", redmondScope);
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
