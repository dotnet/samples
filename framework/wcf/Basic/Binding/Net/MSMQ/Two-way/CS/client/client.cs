//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Messaging;
using System.Net;
using System.ServiceModel;
using System.Transactions;

namespace Microsoft.Samples.MSMQTwoWay
{
    // Define the service contract for order status replies
    [ServiceContract(Namespace = "http://Microsoft.Samples.MSMQTwoWay")]
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


    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Get MSMQ queue name from app settings in configuration
            string queueName = ConfigurationManager.AppSettings["queueName"];

            // Create the transacted MSMQ queue if necessary.
            // This is the queue the order status would be reported to
            if (!MessageQueue.Exists(queueName))
                MessageQueue.Create(queueName, true);

            // Create a ServiceHost for the OrderStatus service type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(OrderStatusService)))
            {

                // Open the ServiceHostBase to create listeners and start listening for order status messages.
                serviceHost.Open();

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

                // Create a client with given client endpoint configuration
                OrderProcessorClient client = new OrderProcessorClient("OrderProcessorEndpoint");

                //Create a transaction scope.
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    string hostName = Dns.GetHostName();

                    // Make a queued call to submit the purchase order
                    client.SubmitPurchaseOrder(po, "net.msmq://" + hostName + "/private/ServiceModelSamplesTwo-way/OrderStatus");

                    // Complete the transaction.
                    scope.Complete();
                }

                //Close down the client
                client.Close();

                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to terminate client.");
                Console.ReadLine();
            }
        }
    }
}

