//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Samples
{
	class FilteringEndpointBehavior : IEndpointBehavior
	{
        MessageFilter addressFilter;
        MessageFilter contractFilter;

        public FilteringEndpointBehavior(MessageFilter addressFilter, MessageFilter contractFilter)
        {
            this.addressFilter = addressFilter;
            this.contractFilter = contractFilter;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            throw new InvalidOperationException("This behavior should only be used on the server.");
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.AddressFilter = this.addressFilter;
            endpointDispatcher.ContractFilter = this.contractFilter;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class FilteringEndpointBehaviorExtension : BehaviorExtensionElement
    {
        public const string ExtensionName = "filteringEndpointBehaviorExtension";

        protected override object CreateBehavior()
        {
            if (Variation == 1)
                return new FilteringEndpointBehavior(new MatchEAddressFilter(), new MatchAllMessageFilter());
            else
                return new FilteringEndpointBehavior(new MatchNoEAddressFilter(), new MatchAllMessageFilter());
        }

        public override Type BehaviorType
        {
            get { return typeof(FilteringEndpointBehavior); }
        }

        [ConfigurationProperty("variation", DefaultValue = 1, IsRequired = true)]
        public int Variation
        {
            get { return (int)base["variation"]; }
            set {
                if (value < 1 || value > 2)
                    throw new ArgumentOutOfRangeException("value", "Variation must be between 1 and 2");
                base["variation"] = value; 
            }
        }
    }
}
