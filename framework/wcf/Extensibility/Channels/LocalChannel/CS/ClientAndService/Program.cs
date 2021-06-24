//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.LocalChannel
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "net.local://localhost:8080/GetPriceService";

            // Start the service host
            ServiceHost host = new ServiceHost(typeof(GetPrice), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IGetPrice), new LocalBinding(), "");
            host.Open();
            Console.WriteLine("In-process service is now running...\n");
            
            // Start the client
            ChannelFactory<IGetPrice> channelFactory
                = new ChannelFactory<IGetPrice>(new LocalBinding(), baseAddress);
            IGetPrice proxy = channelFactory.CreateChannel();

            // Calling in-process service
            Console.WriteLine("Calling in-process service to get the price of product Id {0}: \n\t {1}"
                , 101, proxy.GetPriceForProduct(101));
            Console.WriteLine("Calling in-process service to get the price of product Id {0}: \n\t {1}"
                , 202, proxy.GetPriceForProduct(202));
            Console.WriteLine("Calling in-process service to get the price of product Id {0}: \n\t {1}"
                , 303, proxy.GetPriceForProduct(303));

            Console.WriteLine("\nPress <ENTER> to terminate...");
            Console.ReadLine();
        }
    }
}
