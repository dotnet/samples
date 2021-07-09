//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface IHello
    {
        [OperationContract]
        string Hello();
    }

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            Uri serviceVia = new Uri("http://localhost/ServiceModelSamples/service.svc");
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);
            ChannelFactory<IHello> factory = new ChannelFactory<IHello>(binding, new EndpointAddress(serviceVia));

            Console.WriteLine("Sending message to urn:e...");
            Call(factory, "urn:e", serviceVia);
            Console.WriteLine();
            
            Console.WriteLine("Sending message to urn:a...");
            Call(factory, "urn:a", serviceVia);
            Console.WriteLine();

            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void Call(ChannelFactory<IHello> factory, string address, Uri serviceVia)
        {
            IHello channel = factory.CreateChannel(new EndpointAddress(address), serviceVia);
            try
            {
                string reply = channel.Hello();
                Console.WriteLine(reply.ToString());
                ((IClientChannel)channel).Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("Exception: {0}", ce.Message);
                ((IClientChannel)channel).Abort();
            }
        }

    }
}
