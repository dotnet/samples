//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------(i

using System;
using System.Configuration;
using System.Messaging;
using System.ServiceModel;
using System.Transactions;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Microsoft.Samples.MsmqActivation
{
    // Define the Purchase Order Line Item
    [DataContract(Namespace = "http://Microsoft.Samples.MsmqActivation")]
    public class PurchaseOrderLineItem
    {
        [DataMember]
        public string ProductId;

        [DataMember]
        public float UnitCost;

        [DataMember]
        public int Quantity;

        public override string ToString()
        {
            String displayString = "Order LineItem: " + Quantity + " of " + ProductId + " @unit price: $" + UnitCost + "\n";
            return displayString;
        }

        public float TotalCost
        {
            get { return UnitCost * Quantity; }
        }
    }

    // Define Purchase Order
    [DataContract(Namespace = "http://Microsoft.Samples.MsmqActivation")]
    public class PurchaseOrder
    {
        static string[] OrderStates = { "Pending", "Processed", "Shipped" };
        static Random statusIndexer = new Random(137);

        [DataMember]
        public string PONumber;

        [DataMember]
        public string CustomerId;

        [DataMember]
        public PurchaseOrderLineItem[] orderLineItems;

        public float TotalCost
        {
            get
            {
                float totalCost = 0;
                foreach (PurchaseOrderLineItem lineItem in orderLineItems)
                    totalCost += lineItem.TotalCost;
                return totalCost;
            }
        }

        public string Status
        {
            get
            {
                return OrderStates[statusIndexer.Next(3)];
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder strbuf = new System.Text.StringBuilder("Purchase Order: " + PONumber + "\n");
            strbuf.Append("\tCustomer: " + CustomerId + "\n");
            strbuf.Append("\tOrderDetails\n");

            foreach (PurchaseOrderLineItem lineItem in orderLineItems)
            {
                strbuf.Append("\t\t" + lineItem.ToString());
            }

            strbuf.Append("\tTotal cost of this order: $" + TotalCost + "\n");
            strbuf.Append("\tOrder status: " + Status + "\n");
            return strbuf.ToString();
        }
    }

    // Order Processing Logic
    // Can replace with transaction-aware resource such as SQL, transacted hashtable to hold orders
    // This example uses a non-transactional resource
    public class Orders
    {
        static Dictionary<string, PurchaseOrder> purchaseOrders = new Dictionary<string, PurchaseOrder>();

        public static void Add(PurchaseOrder po)
        {
            purchaseOrders.Add(po.PONumber, po);
        }

        public static string GetOrderStatus(string poNumber)
        {
            PurchaseOrder po;
            if (purchaseOrders.TryGetValue(poNumber, out po))
                return po.Status;
            else
                return null;
        }

        public static void DeleteOrder(string poNumber)
        {
            if (purchaseOrders[poNumber] != null)
                purchaseOrders.Remove(poNumber);
        }
    }

    // Define a service contract. 
    [ServiceContract(Namespace = "http://Microsoft.Samples.MsmqActivation")]
    public interface IOrderProcessor
    {
        [OperationContract(IsOneWay = true)]
        void SubmitPurchaseOrder(PurchaseOrder po, string reportOrderStatusTo);
    }

    // Service class which implements the service contract.
    // Added code to write output to the console window
    public class OrderProcessorService : IOrderProcessor
    {
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void SubmitPurchaseOrder(PurchaseOrder po, string reportOrderStatusTo)
        {
            Orders.Add(po);
            Console.WriteLine("Processing {0} ", po);
            Console.WriteLine("Sending back order status information");
            NetMsmqBinding msmqCallbackBinding = new NetMsmqBinding();
            msmqCallbackBinding.Security.Mode = NetMsmqSecurityMode.None;
            OrderStatusClient client = new OrderStatusClient(msmqCallbackBinding, new EndpointAddress(reportOrderStatusTo));
            // please note that the same transaction that is used to dequeue purchase order is used
            // to send back order status
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                client.OrderStatus(po.PONumber, po.Status);
                scope.Complete();
            }
        }

        /*        // Host the service within this EXE console application for debugging purposes
                public static void Main()
                {
                    // Get the base address that is used to listen for WS-MetaDataExchange requests
                    // This is useful to generate a proxy for the client
                    string baseAddress = ConfigurationManager.AppSettings["baseAddress"];

                    // Create a ServiceHost for the OrderProcessorService type.
                    using (ServiceHost serviceHost = new ServiceHost(new Uri(baseAddress)))
                    {
                        // Open the ServiceHostBase to create listeners and start listening for messages.
                        serviceHost.Open();

                        // The service can now be accessed.
                        Console.WriteLine("The service is ready.");
                        Console.WriteLine("Press <ENTER> to terminate service.");
                        Console.WriteLine();
                        Console.ReadLine();

                    }
                }*/

    }

}

