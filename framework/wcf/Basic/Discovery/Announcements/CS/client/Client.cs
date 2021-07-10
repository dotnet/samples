//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml;

namespace Microsoft.Samples.Discovery
{
    class Client
    {
        public static void Main()
        {
            AnnouncementService announcementService = new AnnouncementService();

            // Subscribe to the announcement events
            announcementService.OnlineAnnouncementReceived += OnOnlineEvent;
            announcementService.OfflineAnnouncementReceived += OnOfflineEvent;

            // Host the AnnouncementService
            ServiceHost announcementServiceHost = new ServiceHost(announcementService);

            try
            {
                // Listen for the announcements sent over UDP multicast
                announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
                announcementServiceHost.Open();

                Console.WriteLine("Listening for service announcements.");
                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to terminate.");
                Console.ReadLine();
                announcementServiceHost.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }   

            if (announcementServiceHost.State != CommunicationState.Closed)
            {
                Console.WriteLine("Aborting the service...");
                announcementServiceHost.Abort();
            }
        }

        static void OnOnlineEvent(object sender, AnnouncementEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("Received an online announcement from {0}:", e.EndpointDiscoveryMetadata.Address);
            PrintEndpointDiscoveryMetadata(e.EndpointDiscoveryMetadata);
        }

        static void OnOfflineEvent(object sender, AnnouncementEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("Received an offline announcement from {0}:", e.EndpointDiscoveryMetadata.Address);
            PrintEndpointDiscoveryMetadata(e.EndpointDiscoveryMetadata);
        }

        static void PrintEndpointDiscoveryMetadata(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            foreach (XmlQualifiedName contractTypeName in endpointDiscoveryMetadata.ContractTypeNames)
            {
                Console.WriteLine("\tContractTypeName: {0}", contractTypeName);
            }
            foreach (Uri scope in endpointDiscoveryMetadata.Scopes)
            {
                Console.WriteLine("\tScope: {0}", scope);
            }
            foreach (Uri listenUri in endpointDiscoveryMetadata.ListenUris)
            {
                Console.WriteLine("\tListenUri: {0}", listenUri);
            }
        }
    }
}

