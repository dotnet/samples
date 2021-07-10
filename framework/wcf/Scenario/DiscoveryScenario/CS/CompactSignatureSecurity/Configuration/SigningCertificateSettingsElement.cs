//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.Discovery.Configuration
{
    
    public sealed class SigningCertificateSettingsElement : ConfigurationElement
    {
        ConfigurationPropertyCollection properties;
        SigningCertificateSettings storeSettings;

        public SigningCertificateSettingsElement()
            : base()
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

        [ConfigurationProperty(ConfigurationStrings.StoreFindType, DefaultValue=X509FindType.FindBySubjectName)]
        public X509FindType CertificatesStoreFindType
        {
            get
            {
                return (X509FindType)base[ConfigurationStrings.StoreFindType];
            }
            set
            {
                base[ConfigurationStrings.StoreFindType] = value;
            }
        }

        [ConfigurationProperty(ConfigurationStrings.StoreIdentifyingValue)]
        public string StoreIdentifyingValue 
        {
            get
            {
                return (string)base[ConfigurationStrings.StoreIdentifyingValue];
            }
            set
            {
                Utility.IfNullThrowNullArgumentException(value, "value");
                base[ConfigurationStrings.StoreIdentifyingValue] = value;
            }
        }

        internal SigningCertificateSettings StoreSettings
        {
            get
            {
                if (this.storeSettings == null)
                {
                    this.storeSettings = new SigningCertificateSettings
                    {
                        StoreLocation = this.CertificatesStoreLocation,
                        StoreName = this.CertificatesStoreName,
                        StoreFindType = this.CertificatesStoreFindType,
                        StoreIdentifyingValue = this.StoreIdentifyingValue
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

                    properties.Add(new ConfigurationProperty(
                                ConfigurationStrings.StoreFindType,
                                typeof(X509FindType),
                                X509FindType.FindBySubjectName,
                                null,
                                null,
                                ConfigurationPropertyOptions.None));

                    properties.Add(new ConfigurationProperty(
                                ConfigurationStrings.StoreIdentifyingValue,
                                typeof(string)));
                    
                    this.properties = properties;
                }

                return this.properties;
            }
        }

        internal void ApplySettings(SigningCertificateSettings signingCertificateStoreSettings)
        {
            Utility.IfNullThrowNullArgumentException(signingCertificateStoreSettings, "signingCertificateStoreSettings");
            this.CertificatesStoreName = signingCertificateStoreSettings.StoreName;
            this.CertificatesStoreLocation = signingCertificateStoreSettings.StoreLocation;
            this.CertificatesStoreFindType = signingCertificateStoreSettings.StoreFindType;
            this.StoreIdentifyingValue = signingCertificateStoreSettings.StoreIdentifyingValue;
        }
    }
}
