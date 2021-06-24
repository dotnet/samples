//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class WorkflowTestClient
    {
        public static void Run(int numberOfMessage)
        {
            try
            {
                int Iteration = numberOfMessage;

                string address = "net.msmq://localhost/private/ReceiveTx";
                NetMsmqBinding binding = new NetMsmqBinding(NetMsmqSecurityMode.None)
                {
                    ExactlyOnce = true,
                    Durable = true
                };

                ChannelFactory<IPurchaseOrderService> channelFactory = new ChannelFactory<IPurchaseOrderService>(binding, address);
                IPurchaseOrderService channel = channelFactory.CreateChannel();

                for (int i = 0; i < Iteration; i++)
                {
                    PurchaseOrder po = new PurchaseOrder()
                    {
                        PONumber = Guid.NewGuid().ToString(),
                        CustomerId = string.Format("CustomerId: {0}", i)
                    };

                    Console.WriteLine("Submitting an PO ...");
                    channel.SubmitPurchaseOrder(po);
                }

                channelFactory.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}