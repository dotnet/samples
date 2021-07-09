//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    /**
     * PingPong Service which demonstrated Client and Service calling each other
     * with a Tick count till the tick count reaches 0.
     */ 
    [ServiceContract(CallbackContract=typeof(IPingPongCallback))]
    public interface IPingPong
    {
        [OperationContract]
        void Ping(int ticks);
    }
    
    public interface IPingPongCallback
    {
        [OperationContract]
        void Pong(int ticks);
    }

    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Reentrant, InstanceContextMode=InstanceContextMode.PerSession)]
    class PingPong : IPingPong
    {
        public static void Main(string[] args)
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(PingPong)))
            {
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

        #region IPingPong Members

        public void Ping(int ticks)
        {
            Console.WriteLine("Ping: Ticks = " + ticks);
            //Keep pinging back and forth till Ticks reaches 0.
            if (ticks != 0)
            {
                OperationContext.Current.GetCallbackChannel<IPingPongCallback>().Pong((ticks - 1));
            }
        }

        #endregion
    }
}
