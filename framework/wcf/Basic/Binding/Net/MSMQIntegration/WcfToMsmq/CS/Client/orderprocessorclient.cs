//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.MsmqIntegration;
using Microsoft.Samples.WCFToMSMQ;

namespace Microsoft.Samples.WCFToMSMQ
{
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://Microsoft.Samples.WCFToMSMQ")]
    public interface IOrderProcessor
    {
        [OperationContract(IsOneWay = true, Action = "*")]
        void SubmitPurchaseOrder(MsmqMessage<PurchaseOrder> msg);
    }

    public partial class OrderProcessorClient : System.ServiceModel.ClientBase<IOrderProcessor>, IOrderProcessor
    {
        public OrderProcessorClient()
        {
        }

        public OrderProcessorClient(string configurationName)
            :
                base(configurationName)
        {
        }

        public OrderProcessorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress address)
            :
                base(binding, address)
        {
        }

        public void SubmitPurchaseOrder(MsmqMessage<PurchaseOrder> msg)
        {
            base.Channel.SubmitPurchaseOrder(msg);
        }
    }
}

