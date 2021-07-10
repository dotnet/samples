//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.MsmqIntegration;
using System.Transactions;

namespace Microsoft.Samples.MSMQMessageCorrelation
{
    // Define a service contract. 
    [ServiceContract(Namespace = "http://Microsoft.Samples.MSMQMessageCorrelation")]
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
            PurchaseOrder po = (PurchaseOrder)ordermsg.Body;
            Random statusIndexer = new Random();
            po.Status = (OrderStates)statusIndexer.Next(3);
            Console.WriteLine("Processing {0} ", po);
            //Send a response to the client that the order has been received and is pending fullfillment 
            SendResponse(ordermsg);
        }

        private void SendResponse(MsmqMessage<PurchaseOrder> ordermsg)
        {
            OrderResponseClient client = new OrderResponseClient("OrderResponseEndpoint");
            
            //Set the correlation ID such that the client can correlate the response to the order
            MsmqMessage<PurchaseOrder> orderResponsemsg = new MsmqMessage<PurchaseOrder>(ordermsg.Body);
            orderResponsemsg.CorrelationId = ordermsg.Id;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                client.SendOrderResponse(orderResponsemsg);
                scope.Complete();
            }
            client.Close();
        }
    }
}

