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

    public class UdpSecureAnnouncementEndpointElement : UdpAnnouncementEndpointElement
    {
        ConfigurationPropertyCollection properties;

        public UdpSecureAnnouncementEndpointElement()
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
            get { return typeof(UdpSecureAnnouncementEndpoint); }
        }

        protected override ServiceEndpoint CreateServiceEndpoint(ContractDescription contractDescription)
        {
            UdpSecureAnnouncementEndpoint endpoint = new UdpSecureAnnouncementEndpoint(
               this.DiscoveryVersion,
               this.SigningCertificateStoreSettingsElement.StoreSettings);
            endpoint.ReceivedCertificatesStoreSettings = this.ReceivedCertificatesStoreSettingsElement.StoreSettings;
            return endpoint;
        }

        protected override void InitializeFrom(ServiceEndpoint endpoint)
        {
            base.InitializeFrom(endpoint);

            UdpSecureAnnouncementEndpoint source = (UdpSecureAnnouncementEndpoint)endpoint;
            this.ReceivedCertificatesStoreSettingsElement.ApplySettings(source.ReceivedCertificatesStoreSettings);
            this.SigningCertificateStoreSettingsElement.ApplySettings(source.SigningCertificateStoreSettings);
            this.DiscoveryVersion = source.DiscoveryVersion;
        }

        ChannelEndpointElement GetDefaultAnnouncementEndpointElement()
        {
            return new ChannelEndpointElement() { Kind = ConfigurationStrings.UdpSecureAnnouncementEndpoint };
        }
    }
}
