//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.CustomChannelDispatcher
{
    [ServiceContract(Name = "HelloWorld")]
    public interface IHelloWorld
    {
        [OperationContract(IsOneWay = false)]
        string Hello(string greeting);
    }

    public class Program
    {
        const string serviceUrl = "http://localhost/ServiceModelSamples/Service.svc";
        int count = 5;

        public static void Main(String[] args)
        {
            Program p = new Program();
            p.Run();
        }

        void Run()
        {
            Uri uri = new Uri(serviceUrl);
            Console.WriteLine("Client is talking to a RequestReply WCF service.");

            ChannelFactory<IHelloWorld> channelFactory = new ChannelFactory<IHelloWorld>(new BasicHttpBinding());
            IHelloWorld clientService = channelFactory.CreateChannel(new EndpointAddress(uri));

            Console.Write("Type what you want to say to the server: ");
            string greeting = Console.ReadLine();

            for (int i = 0; i < count; i++)
            {
                string reply = clientService.Hello(greeting);
                Console.WriteLine("Server replied: {0}", reply);
            }

            ((IChannel)clientService).Close();
            channelFactory.Close();

            Console.ReadLine();
        }
    }
}
