//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.MsmqIntegration;

namespace Microsoft.Samples.MSMQToWCF
{
    // Define a service contract. 
    [ServiceContract(Namespace = "http://Microsoft.Samples.MSMQToWCF")]
    [ServiceKnownType(typeof(PurchaseOrder))]
    public interface IOrderProcessor
    {
        [OperationContract(IsOneWay = true, Action = "*")]
        void SubmitPurchaseOrder(MsmqMessage<PurchaseOrder> msg);
    }

    // Service class which implements the service contract.
    // Added code to write output to the console window
    public class OrderProcessorService : IOrderProcessor
    {
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void SubmitPurchaseOrder(MsmqMessage<PurchaseOrder> ordermsg)
        {
            PurchaseOrder po = ordermsg.Body;
            Random statusIndexer = new Random();
            po.Status = (OrderStates)statusIndexer.Next(3);
            Console.WriteLine("Processing {0} ", po);
        }

        // Host the service within this EXE console application.
        public static void Main()
        {
            // Get MSMQ queue name from app settings in configuration
            string queueName = ConfigurationManager.AppSettings["orderQueueName"];

            // Create the transacted MSMQ queue if necessary.
            if (!MessageQueue.Exists(queueName))
                MessageQueue.Create(queueName, true);

            // Create a ServiceHost for the OrderProcessorService type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(OrderProcessorService)))
            {
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.ReadLine();
            }
        }
    }
}


