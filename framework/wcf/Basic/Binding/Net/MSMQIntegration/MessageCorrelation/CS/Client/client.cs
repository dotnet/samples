//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Messaging;
using System.Transactions;

namespace Microsoft.Samples.MSMQMessageCorrelation
{
    // Define the Purchase Order Line Item
    
    class Program
	{
        static string orderMessageID;
        static void DisplayOrderStatus()
        {
            MessageQueue orderResponseQueue = new MessageQueue(ConfigurationManager.AppSettings["orderResponseQueueName"]);

            //Create a transaction scope.
            bool responseReceived = false;
            orderResponseQueue.MessageReadPropertyFilter.CorrelationId = true;
            while (!responseReceived)
            {
                Message responseMsg;
                using (TransactionScope scope2 = new TransactionScope(TransactionScopeOption.Required))
                {
                    //Receive the Order Response message


                    responseMsg = orderResponseQueue.Receive(MessageQueueTransactionType.Automatic);
                    scope2.Complete();
                }
                responseMsg.Formatter = new System.Messaging.XmlMessageFormatter(new Type[] { typeof(PurchaseOrder) });
                PurchaseOrder responsepo = (PurchaseOrder)responseMsg.Body;
                //Check if the response is for the order placed
                if (orderMessageID == responseMsg.CorrelationId)
                {
                    responseReceived = true;
                    Console.WriteLine("Status of current Order: OrderID-{0},Order Status-{1}", responsepo.poNumber, responsepo.Status);
                }
                else
                {
                    Console.WriteLine("Status of previous Order: OrderID-{0},Order Status-{1}", responsepo.poNumber, responsepo.Status);
                }
            }
        }

        static void PlaceOrder()
        {
            //Connect to the queue
            MessageQueue orderQueue = new MessageQueue("FormatName:Direct=OS:" + ConfigurationManager.AppSettings["orderQueueName"]);

            // Create the purchase order
            PurchaseOrder po = new PurchaseOrder();
            po.customerId = "somecustomer.com";
            po.poNumber = Guid.NewGuid().ToString();

            PurchaseOrderLineItem lineItem1 = new PurchaseOrderLineItem();
            lineItem1.productId = "Blue Widget";
            lineItem1.quantity = 54;
            lineItem1.unitCost = 29.99F;

            PurchaseOrderLineItem lineItem2 = new PurchaseOrderLineItem();
            lineItem2.productId = "Red Widget";
            lineItem2.quantity = 890;
            lineItem2.unitCost = 45.89F;

            po.orderLineItems = new PurchaseOrderLineItem[2];
            po.orderLineItems[0] = lineItem1;
            po.orderLineItems[1] = lineItem2;

            Message msg = new Message();
            msg.UseDeadLetterQueue = true;
            msg.Body = po;

            //Create a transaction scope.
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                // submit the purchase order
                orderQueue.Send(msg, MessageQueueTransactionType.Automatic);
                // Complete the transaction.
                scope.Complete();
                
            }
            //Save the string for orderResponse correlation
            orderMessageID = msg.Id;
            Console.WriteLine("Placed the order, waiting for response...");
        }

        static void Main(string[] args)
		{
            // Create the transacted response queue if necessary.
            if (!MessageQueue.Exists(ConfigurationManager.AppSettings["orderResponseQueueName"]))
                MessageQueue.Create(ConfigurationManager.AppSettings["orderResponseQueueName"], true);

			//Place an order
            PlaceOrder();
            DisplayOrderStatus();
            Console.WriteLine("Press enter to terminate the client");
            Console.ReadLine();
		}
	}
}

