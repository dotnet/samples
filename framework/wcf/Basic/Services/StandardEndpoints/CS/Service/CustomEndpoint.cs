
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.Samples.StandardEndpoints
{

    public class CustomEndpoint : ServiceEndpoint
    {
        public CustomEndpoint()
            : this(string.Empty)
        {
        }

        public CustomEndpoint(string address)
            : this(address, ContractDescription.GetContract(typeof(ICalculator)))
        {
        }

        // Create the custom endpoint with a fixed binding
        public CustomEndpoint(string address, ContractDescription contract)
            : base(contract)
        {
            this.Binding = new BasicHttpBinding();
            this.IsSystemEndpoint = false;
        }

        // Definition of the additional property of this endpoint
        public bool Property
        {
            get;
            set;
        }
    }
}
