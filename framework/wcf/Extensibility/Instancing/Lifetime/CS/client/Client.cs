//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract(SessionMode=SessionMode.Required)]
    interface IEchoService
    {
        [OperationContract]
        string Echo(string value);
    }

    public static class CustomHeader
    {
        public static readonly String HeaderName = "InstanceId";
        public static readonly String HeaderNamespace = "http://Microsoft.ServiceModel.Samples/Lifetime";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "Press <enter> to open a channel and send a request.");

            Console.ReadLine();
            
            MessageHeader shareableInstanceContextHeader = MessageHeader.CreateHeader(
                    CustomHeader.HeaderName,
                    CustomHeader.HeaderNamespace,
                    Guid.NewGuid().ToString());

            ChannelFactory<IEchoService> channelFactory =
                new ChannelFactory<IEchoService>("echoservice");
            
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
