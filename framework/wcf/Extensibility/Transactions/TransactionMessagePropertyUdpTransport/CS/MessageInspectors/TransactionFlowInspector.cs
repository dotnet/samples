//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Transactions;

namespace Microsoft.ServiceModel.Samples
{
    class TransactionFlowInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            // obtain the tx propagation token
            byte[] propToken = null;           
            if (Transaction.Current != null && IsTxFlowRequiredForThisOperation(request.Headers.Action))
            {
                try
                {
                    propToken = TransactionInterop.GetTransmitterPropagationToken(Transaction.Current);
                }
                catch (TransactionException e)
                {
                    throw new CommunicationException("TransactionInterop.GetTransmitterPropagationToken failed.", e);
                }
            }

            // set the propToken on the message in a TransactionFlowProperty
            TransactionFlowProperty.Set(propToken, request);

            return null;            
        }

        static bool IsTxFlowRequiredForThisOperation(String action)
        {
            // In general, this should contain logic to identify which operations (actions) require transaction flow.
            // Here we just flow transactions for all actions.
            return true;
        }
    }
}
