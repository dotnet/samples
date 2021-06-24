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
        public static void Main(string[] args)
        {
            Console.WriteLine(" **** Announcement service ****");
            AnnouncementService announcementService = new AnnouncementService();
            announcementService.OnlineAnnouncementReceived += new EventHandler<AnnouncementEventArgs>(announcementService_OnlineAnnouncementReceived);
            announcementService.OfflineAnnouncementReceived += new EventHandler<AnnouncementEventArgs>(announcementService_OfflineAnnouncementReceived);

            // Search for the signing certificate in StoreLocation.LocalMachine, StoreName.My, using FindBySubjectDistinguishedName
            // Keep the default settings for client certificates (used to validate incoming messages): StoreLocation.LocalMachine, StoreName.My
            SigningCertificateSettings signingStoreSettings = new SigningCertificateSettings("CN=DiscoverySecureServiceCertificate");

            ServiceHost host = new ServiceHost(announcementService);

            UdpAnnouncementEndpoint announcementEndpoint = new UdpSecureAnnouncementEndpoint(
                DiscoveryVersion.WSDiscovery11,
                signingStoreSettings);

            try
            {
                host.AddServiceEndpoint(announcementEndpoint);
                host.Open();

                Console.WriteLine("\nPress enter to exit ..");
                Console.ReadLine();
                host.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }

            if (host.State != CommunicationState.Closed)
            {
                Console.WriteLine("Aborting the service...");
                host.Abort();
            }
        }

        static void announcementService_OfflineAnnouncementReceived(object sender, AnnouncementEventArgs e)
        {
            Console.WriteLine("Received Offline Announcement from {0}", e.EndpointDiscoveryMetadata.Address);
        }

        static void announcementService_OnlineAnnouncementReceived(object sender, AnnouncementEventArgs e)
        {
            Console.WriteLine("Received Online Announcement from {0}", e.EndpointDiscoveryMetadata.Address);
        }
    }
}
