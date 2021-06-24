//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Diagnostics;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract]
    interface IPoolService
    {
        [OperationContract]
        string GetData();

        [OperationContract]
        string GetDataNoPooling();
    }

    class Client
    {
        static void Main(string[] args)
        {
            CallPoolService();
	    Console.WriteLine("Press <ENTER> to complete test.");
	    Console.ReadLine();          
        }

        static void CallPoolService()
        {           
            ChannelFactory<IPoolService> channelFactory =
                new ChannelFactory<IPoolService>("poolservice");

            IPoolService proxy = channelFactory.CreateChannel();
            
            Console.WriteLine("Press <ENTER> to invoke the method which returns the object " +
                "to the pool.");
            Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(proxy.GetData());
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("Press <ENTER> to invoke the method which does not return " +
                "the object to the pool.");
            Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(proxy.GetDataNoPooling());
            Console.ForegroundColor = ConsoleColor.Gray;  
        }        
    }
}
