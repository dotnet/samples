//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.MSMQDeadLetter
{
    // Service class which implements the service contract.
    // Added code to write output to the console window
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Single, AddressFilterMode=AddressFilterMode.Any)]
    public class PurchaseOrderDLQService : IOrderProcessor
    {
        OrderProcessorClient orderProcessorService;
        public PurchaseOrderDLQService()
        {
            orderProcessorService = new OrderProcessorClient("OrderProcessorEndpoint");
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void SubmitPurchaseOrder(PurchaseOrder po)
        {
            Console.WriteLine("Submitting purchase order did not succeed ", po);
            MsmqMessageProperty mqProp = OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;

            Console.WriteLine("Message Delivery Status: {0} ", mqProp.DeliveryStatus);
            Console.WriteLine("Message Delivery Failure: {0}", mqProp.DeliveryFailure);
            Console.WriteLine();

            // resend the message if timed out
            if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
                mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
            {
                // re-send
                Console.WriteLine("Purchase order Time To Live expired");
                Console.WriteLine("Trying to resend the message");
                
                // reuse the same transaction used to read the message from dlq to enqueue the message to app. queue
                orderProcessorService.SubmitPurchaseOrder(po);
                Console.WriteLine("Purchase order resent");
            }
        }
    
        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the PurchaseOrderDLQService type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(PurchaseOrderDLQService)))
            {
                // Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The dead letter service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

    }

}

