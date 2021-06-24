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
            Uri baseAddress = new Uri("net.tcp://localhost:8001/DiscoveryRouter/");            

            ServiceHost serviceHost = new ServiceHost(new DiscoveryRoutingService(new UdpDiscoveryEndpoint()), baseAddress);

            try
            {
                DiscoveryEndpoint discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8001/DiscoveryRouter/"));
                discoveryEndpoint.IsSystemEndpoint = false;

                serviceHost.AddServiceEndpoint(discoveryEndpoint);

                serviceHost.Open();

                Console.WriteLine("Discovery Routing Service started at {0}", baseAddress);
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
