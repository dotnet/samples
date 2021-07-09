//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Messaging;
using System.ServiceModel;
using System.Transactions;

namespace Microsoft.Samples.MsmqActivation
{
    // Define the service contract for order status replies
    [ServiceContract(Namespace="http://Microsoft.Samples.MsmqActivation")]
    public interface IOrderStatus
    {
        [OperationContract(IsOneWay = true)]
        void OrderStatus(string poNumber, string status);
    }

    // Service that handles order status
    [ServiceBehavior]
    public class OrderStatusService : IOrderStatus
    {
        [OperationBehavior(TransactionAutoComplete = true, TransactionScopeRequired = true)]
        public void OrderStatus(string poNumber, string status)
        {
            Console.WriteLine("Status of order {0}:{1} ", poNumber, status);
        }
    }


    //The service contract is defined in generatedProxy.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Get MSMQ queue name from app settings in configuration
            string targetQueueName = ConfigurationManager.AppSettings["targetQueueName"];

            // Create the transacted MSMQ queue if necessary.
            // This is the queue the order status would be reported to
            if (!MessageQueue.Exists(targetQueueName))
                MessageQueue.Create(targetQueueName, true);

            // Get MSMQ queue name from app settings in configuration
            string responseQueueName = ConfigurationManager.AppSettings["responseQueueName"];

            // Create the transacted MSMQ queue if necessary.
            // This is the queue the order status would be reported to
            if (!MessageQueue.Exists(responseQueueName))
                MessageQueue.Create(responseQueueName, true);


            // Create a ServiceHost for the OrderStatus service type.
            ServiceHost serviceHost = new ServiceHost(typeof(OrderStatusService));

            // Open the ServiceHostBase to create listeners and start listening for order status messages.
            serviceHost.Open();

            // Create a proxy with given client endpoint configuration
            OrderProcessorClient client = new OrderProcessorClient();

            // Create the purchase order
            PurchaseOrder po = new PurchaseOrder();
            po.CustomerId = "somecustomer.com";
            po.PONumber = Guid.NewGuid().ToString();

            PurchaseOrderLineItem lineItem1 = new PurchaseOrderLineItem();
            lineItem1.ProductId = "Blue Widget";
            lineItem1.Quantity = 54;
            lineItem1.UnitCost = 29.99F;

            PurchaseOrderLineItem lineItem2 = new PurchaseOrderLineItem();
            lineItem2.ProductId = "Red Widget";
            lineItem2.Quantity = 890;
            lineItem2.UnitCost = 45.89F;

            po.orderLineItems = new PurchaseOrderLineItem[2];
            po.orderLineItems[0] = lineItem1;
            po.orderLineItems[1] = lineItem2;

            //Create a transaction scope.
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                // Make a queued call to submit the purchase order
                client.SubmitPurchaseOrder(po, "net.msmq://localhost/private/ServiceModelSamplesOrder/OrderStatus");
                // Complete the transaction.
                scope.Complete();
            }

            //Closing the client gracefully closes the connection and cleans up resources
            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();

            client.Close();

            try
            {
                serviceHost.Close();
            }
            catch (CommunicationObjectFaultedException)
            {
                Console.WriteLine("Warning: the order status service is in faulted state");
            }
        }
    }
}


