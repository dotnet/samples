//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.ReliableSecureProfile
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceAddress = "http://localhost/ProcessDataService";

            CustomBinding rspBinding = new CustomBinding();
            rspBinding.Elements.Add(new ReliableSessionBindingElement());
            rspBinding.Elements.Add(new MakeConnectionBindingElement());
            rspBinding.Elements.Add(new TextMessageEncodingBindingElement());
            rspBinding.Elements.Add(new HttpTransportBindingElement());

            DuplexChannelFactory<IProcessDataDuplex> channelFactory =
                new DuplexChannelFactory<IProcessDataDuplex>
                    (new CallbackHandler(), rspBinding, serviceAddress);
            IProcessDataDuplex client = channelFactory.CreateChannel();
            Console.Write("Enter string to compute hash : ");
            string rawData = Console.ReadLine();
            Console.WriteLine("Calling service now...");
            client.ProcessData(rawData);
            Console.WriteLine("Computing of hash has started on the service side...");
            Console.ReadLine();
        }

        public class CallbackHandler : IProcessDataDuplexCallBack
        {
            public void SendProcessedData(string processedData)
            {
                Console.WriteLine("Processing of data on the service side has ended...");
                Console.WriteLine("This is service sending data to the client with no listener open on the client...");
                Console.WriteLine(processedData);
                Console.WriteLine("\n\nPress <ENTER> to terminate the client.");
                Console.ReadLine();
            }
        }
    }
}
