//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    [ServiceContract]
    public interface IPurchaseOrderService
    {
        [OperationContract(IsOneWay = true)]
        void SubmitPurchaseOrder(PurchaseOrder po);
    }

    [DataContract]
    public class PurchaseOrder
    {
        [DataMember]
        public string PONumber { get; set; }

        [DataMember]
        public string CustomerId { get; set; }

        [DataMember]
        public PurchaseOrderLineItem[] OrderLineItems { get; set; }
    }

    [DataContract]
    public class PurchaseOrderLineItem
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public float UnitCost { get; set; }

        [DataMember]
        public int Quantity { get; set; }
    }
}