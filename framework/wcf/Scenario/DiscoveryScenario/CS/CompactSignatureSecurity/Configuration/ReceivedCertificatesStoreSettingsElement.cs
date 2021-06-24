//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.Discovery.Configuration
{
    
    public sealed class ReceivedCertificatesStoreSettingsElement : ConfigurationElement
    {
        ConfigurationPropertyCollection properties;
        ReceivedCertificatesStoreSettings storeSettings;

        public ReceivedCertificatesStoreSettingsElement() : base()
        {
        }

        [ConfigurationProperty(ConfigurationStrings.StoreLocation, DefaultValue=StoreLocation.LocalMachine)]
        public StoreLocation CertificatesStoreLocation
        {
            get
            {
                return (StoreLocation)base[ConfigurationStrings.StoreLocation];
            }
            set
            {
                base[ConfigurationStrings.StoreLocation] = value;
            }
        }

        [ConfigurationProperty(ConfigurationStrings.StoreName, DefaultValue=StoreName.TrustedPeople)]
        public StoreName CertificatesStoreName
        {
            get
            {
                return (StoreName)base[ConfigurationStrings.StoreName];
            }
            set
            {
                base[ConfigurationStrings.StoreName] = value;
            }
        }

        internal ReceivedCertificatesStoreSettings StoreSettings
        {
            get
            {
                if (this.storeSettings == null)
                {
                    this.storeSettings = new ReceivedCertificatesStoreSettings
                    {
                        StoreLocation = this.CertificatesStoreLocation,
                        StoreName = this.CertificatesStoreName
                    };
                }
                return this.storeSettings;
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
                                ConfigurationStrings.StoreLocation, 
                                typeof(StoreLocation), 
                                StoreLocation.LocalMachine,
                                null,
                                null,
                                ConfigurationPropertyOptions.None));

                    properties.Add(new ConfigurationProperty(
                                ConfigurationStrings.StoreName,
                                typeof(StoreName),
                                StoreName.TrustedPeople,
                                null,
                                null,
                                ConfigurationPropertyOptions.None));
                    
                    this.properties = properties;
                }

                return this.properties;
            }
        }
                
        internal void ApplySettings(ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings)
        {
            Utility.IfNullThrowNullArgumentException(receivedCertificatesStoreSettings, "receivedCertificatesStoreSettings");
            this.CertificatesStoreName = receivedCertificatesStoreSettings.StoreName;
            this.CertificatesStoreLocation = receivedCertificatesStoreSettings.StoreLocation;
        }
    }
}
