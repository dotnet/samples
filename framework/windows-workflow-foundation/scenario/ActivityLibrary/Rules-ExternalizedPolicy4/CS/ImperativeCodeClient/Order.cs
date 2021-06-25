//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Samples.Activities.Rules.Client
{
    public enum CustomerType
    {
        Residential,
        Business
    }

    public class Order
    {
        private double discount;
        private double orderValue;
        private CustomerType customerType;

        public double Discount 
        { 
            get { return this.discount; }  
            set { this.discount = value; }
        }

        public double OrderValue 
        {
            get { return this.orderValue; }
            set { this.orderValue = value; }
        }

        public CustomerType CustomerType 
        { 
            get { return this.customerType; }  
            set { this.customerType = value; }
        }

        public Order() 
        { 
        }

        public Order(double orderValue, CustomerType customerType)
        {
            this.OrderValue = orderValue;
            this.CustomerType = customerType;
        }

        public override string ToString()
        {
            return string.Format("Value: {0}, Type: {1}, Discount: {2}", this.orderValue, this.customerType.ToString(), this.discount);
        }
    }
}
