//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Samples.LinqMessageQueryCorrelation.SharedTypes
{    
    [DataContract (Name="PurchaseOrder", Namespace=Constants.DefaultNamespace)]
    public class PurchaseOrder
    {
        private int id;
        private string partname;
        private int quantity;
        private int customerid;

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string PartName
        {
            get { return partname; }
            set { partname = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [DataMember]
        public int CustomerId
        {
            get { return customerid; }
            set { customerid = value; }
        }              
    }

    [DataContract(Name = "OrderStatus", Namespace = Constants.DefaultNamespace)]
    public class OrderStatus
    {
        private int id;
        private bool confirmed;

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public bool Confirmed
        {
            get { return confirmed; }
            set { confirmed = value; }
        }
    }
}
