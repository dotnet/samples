
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace Microsoft.Samples.StandardEndpoints
{

    public class CustomEndpointElement : StandardEndpointElement
    {
        // Definition of the additional property for the standard endpoint element
        public bool Property
        {
            get { return (bool)base["property"]; }
            set { base["property"] = value; }
        }

        // The additional property needs to be added to the properties of the standard endpoint element
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection properties = base.Properties;
                properties.Add(new ConfigurationProperty("property", typeof(bool), false, ConfigurationPropertyOptions.None));
                return properties;
            }
        }

        // Return the type of this standard endpoint
        protected override Type EndpointType
        {
            get { return typeof(CustomEndpoint); }
        }

        // Create the custom service endpoint
        protected override ServiceEndpoint CreateServiceEndpoint(ContractDescription contract)
        {
            return new CustomEndpoint();
        }

        // Read the value given to the property in config and save it
        protected override void OnApplyConfiguration(ServiceEndpoint endpoint, ServiceEndpointElement serviceEndpointElement)
        {
            CustomEndpoint customEndpoint = (CustomEndpoint)endpoint;
            customEndpoint.Property = this.Property;
        }

        // Read the value given to the property in config and save it
        protected override void OnApplyConfiguration(ServiceEndpoint endpoint, ChannelEndpointElement channelEndpointElement)
        {
            CustomEndpoint customEndpoint = (CustomEndpoint)endpoint;
            customEndpoint.Property = this.Property;
        }

        // No validation in this sample
        protected override void OnInitializeAndValidate(ServiceEndpointElement serviceEndpointElement)
        {
        }

        // No validation in this sample
        protected override void OnInitializeAndValidate(ChannelEndpointElement channelEndpointElement)
        {
        }
    }
}

