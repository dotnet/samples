//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.MsmqIntegration;

namespace Microsoft.Samples.MSMQMessageCorrelation
{
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://Microsoft.Samples.MSMQMessageCorrelation")]
    public interface IOrderResponse
    {

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "*")]
        void SendOrderResponse(MsmqMessage<PurchaseOrder> msg);
    }

    public partial class OrderResponseClient : System.ServiceModel.ClientBase<IOrderResponse>, IOrderResponse
    {

        public OrderResponseClient()
        { }

        public OrderResponseClient(string configurationName)
            : base(configurationName)
        { }

        public OrderResponseClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress address)
            : base(binding, address)
        { }

        public void SendOrderResponse(MsmqMessage<PurchaseOrder> msg)
        {
            base.Channel.SendOrderResponse(msg);
        }
    }
}

