//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery.Configuration;

namespace Microsoft.Samples.Discovery.Configuration
{

    public class UdpSecureDiscoveryEndpointElement : UdpDiscoveryEndpointElement
    {
        ConfigurationPropertyCollection properties;

        public UdpSecureDiscoveryEndpointElement()
            : base()
        {
        }

        [ConfigurationProperty(ConfigurationStrings.ReceivedCertificatesStoreSettings)]
        public ReceivedCertificatesStoreSettingsElement ReceivedCertificatesStoreSettingsElement
        {
            get
            {
                return (ReceivedCertificatesStoreSettingsElement)base[ConfigurationStrings.ReceivedCertificatesStoreSettings];
            }
            set
            {
                base[ConfigurationStrings.ReceivedCertificatesStoreSettings] = value;
            }
        }

        [ConfigurationProperty(ConfigurationStrings.SigningCertificateSettings)]
        public SigningCertificateSettingsElement SigningCertificateStoreSettingsElement
        {
            get
            {
                return (SigningCertificateSettingsElement)base[ConfigurationStrings.SigningCertificateSettings];
            }
            set
            {
                base[ConfigurationStrings.SigningCertificateSettings] = value;
            }
        }
       
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;

                    properties.Add(new ConfigurationProperty(
                        ConfigurationStrings.SigningCertificateSettings, typeof(SigningCertificateSettingsElement)));
                    properties.Add(new ConfigurationProperty(
                        ConfigurationStrings.ReceivedCertificatesStoreSettings, typeof(ReceivedCertificatesStoreSettingsElement)));

                    this.properties = properties;
                }
                return this.properties;
            }
        }

        protected override Type EndpointType
        {
            get { return typeof(UdpSecureDiscoveryEndpoint); }
        }

        protected override ServiceEndpoint CreateServiceEndpoint(ContractDescription contractDescription)
        {
            UdpSecureDiscoveryEndpoint endpoint = new UdpSecureDiscoveryEndpoint(
                this.DiscoveryVersion,
                this.SigningCertificateStoreSettingsElement.StoreSettings);
            endpoint.ReceivedCertificatesStoreSettings = this.ReceivedCertificatesStoreSettingsElement.StoreSettings;
            return endpoint;
        }

        protected override void InitializeFrom(ServiceEndpoint endpoint)
        {
            base.InitializeFrom(endpoint);

            UdpSecureDiscoveryEndpoint source = (UdpSecureDiscoveryEndpoint)endpoint;
            this.ReceivedCertificatesStoreSettingsElement.ApplySettings(source.ReceivedCertificatesStoreSettings);
            this.SigningCertificateStoreSettingsElement.ApplySettings(source.SigningCertificateStoreSettings);
            this.DiscoveryVersion = source.DiscoveryVersion;
        }

        ChannelEndpointElement GetDefaultDiscoveryEndpointElement()
        {
            return new ChannelEndpointElement() { Kind = ConfigurationStrings.UdpSecureDiscoveryEndpoint };
        }
    }
}
