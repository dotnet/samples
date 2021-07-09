//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Transactions;

namespace Microsoft.Samples.MSMQSessionSample
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            //Create a transaction scope.
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                // Create a client with given client endpoint configuration
                OrderTakerClient client = new OrderTakerClient("OrderTakerEndpoint");
                // Open a purchase order
                client.OpenPurchaseOrder("somecustomer.com");
                Console.WriteLine("Purchase Order created");

                // Add product line items
                Console.WriteLine("Adding 10 quantities of blue widget");
                client.AddProductLineItem("Blue Widget", 10);

                Console.WriteLine("Adding 23 quantities of red widget");
                client.AddProductLineItem("Red Widget", 23);

                // Close the purchase order
                Console.WriteLine("Closing the purchase order");
                client.EndPurchaseOrder();

                //Closing the client gracefully closes the connection and cleans up resources
                client.Close();                

                // Complete the transaction.
                scope.Complete();
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}


