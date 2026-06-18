//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    internal interface IEchoService
    {
        [OperationContract]
        string Echo(string value);
    }

    public static class CustomHeader
    {
        public const string HeaderName = "InstanceId";
        public const string HeaderNamespace = "http://Microsoft.ServiceModel.Samples/Lifetime";
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press <enter> to open a channel and send a request.");

            Console.ReadLine();

            MessageHeader shareableInstanceContextHeader = MessageHeader.CreateHeader(
                    CustomHeader.HeaderName,
                    CustomHeader.HeaderNamespace,
                    Guid.NewGuid().ToString());

#if NET6_0_OR_GREATER
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(), new TcpTransportBindingElement());
            EndpointAddress endpointAddress = new EndpointAddress(new Uri("net.tcp://localhost:9000/echoservice"));
            ChannelFactory<IEchoService> channelFactory = new ChannelFactory<IEchoService>(binding, endpointAddress);
#else
            ChannelFactory<IEchoService> channelFactory = new ChannelFactory<IEchoService>("echoservice");
#endif
            IEchoService proxy = channelFactory.CreateChannel();

            using (new OperationContextScope((IClientChannel)proxy))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(shareableInstanceContextHeader);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Service returned: " + proxy.Echo("Apple"));
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            ((IChannel)proxy).Close();

            Console.WriteLine("Channel No 1 closed.");

            Console.WriteLine(
                "Press <ENTER> to send another request from a different channel " +
                "to the same instance. ");

            Console.ReadLine();

            proxy = channelFactory.CreateChannel();

            using (new OperationContextScope((IClientChannel)proxy))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(shareableInstanceContextHeader);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Service returned: " + proxy.Echo("Apple"));
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            ((IChannel)proxy).Close();

            Console.WriteLine("Channel No 2 closed.");

            Console.WriteLine("Press <ENTER> to complete test.");
            Console.ReadLine();
        }
    }
}
