
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using Microsoft.ServiceModel.Samples;

namespace Microsoft.ServiceModel.Samples
{
    /**
     * The callback implementation calls back to the service and so this also must
     * be marked Reentrant.
     */ 
    [CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Reentrant)]
    public class PingPongCallback : IPingPongCallback
    {
        #region IPingPongCallback Members

        public void Pong(int ticks)
        {
            Console.WriteLine("Pong: Ticks = " + ticks);
            if (ticks != 0)
            {
                //Retrieve the Callback  Channel (in this case the Channel that was used to send the
                //original message) and make an outgoing call until ticks reaches 0.
                IPingPong channel = OperationContext.Current.GetCallbackChannel<IPingPong>();
                channel.Ping((ticks - 1));
            }
        }

        #endregion
    }

    public class Client
    {
        public static void Main(String[] args)
        {
            //Default is to PingPong between client and server twice
            int ticks = (args.Length == 0) ? 10 : Convert.ToInt32(args[0]);
            //Create a PingPong client
            PingPongClient pingPongClient = new PingPongClient(new InstanceContext(new PingPongCallback()));
            pingPongClient.Open();
            pingPongClient.Ping(ticks);
            pingPongClient.Close();
            
            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
