//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Messaging;
using System.Transactions;

namespace Microsoft.Samples.MSMQDeadLetter
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Get MSMQ queue name from app settings in configuration
            string deadLetterQueueName = ConfigurationManager.AppSettings["deadLetterQueueName"];

            // Create the transacted MSMQ queue for storing dead message if necessary.
            if (!MessageQueue.Exists(deadLetterQueueName))
                MessageQueue.Create(deadLetterQueueName, true);

            // Create a proxy with given client endpoint configuration
            OrderProcessorClient client = new OrderProcessorClient("OrderProcessorEndpoint");

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
                client.SubmitPurchaseOrder(po);
                // Complete the transaction.
                scope.Complete();
            }

            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}

