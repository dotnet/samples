//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.Discovery
{

    /// <summary>
    /// Stores the details of the store where the signing certificate is found.
    /// </summary>
    public class SigningCertificateSettings
    {
        public SigningCertificateSettings()
        {
            this.Initialize();
            this.StoreIdentifyingValue = string.Empty;
        }
        
        public SigningCertificateSettings(string storeIdentifyingValue)
        {
            this.Initialize();
            this.StoreIdentifyingValue = storeIdentifyingValue;
        }

        internal SigningCertificateSettings(SigningCertificateSettings signingStoreSettings)
        {
            Utility.IfNullThrowNullArgumentException(signingStoreSettings, "signingStoreSettings");
            this.StoreLocation = signingStoreSettings.StoreLocation;
            this.StoreFindType = signingStoreSettings.StoreFindType;
            this.StoreName = signingStoreSettings.StoreName;
            this.StoreIdentifyingValue = signingStoreSettings.StoreIdentifyingValue;
        }

        public StoreLocation StoreLocation { get; set; }

        public StoreName StoreName { get; set; }

        public X509FindType StoreFindType { get; set; }

        public string StoreIdentifyingValue { get; set; }

        // The signing certificate, computed based on signing store settings.
        public X509Certificate2 GetSigningCertificate()
        {
            Utility.IfNullThrowNullArgumentException(this.StoreIdentifyingValue, "signingStoreIdentifyingValue");

            X509Certificate2 certificate = CertificateHelper.GetCertificate(
                this.StoreName,
                this.StoreLocation,
                this.StoreFindType,
                this.StoreIdentifyingValue);

            // GetCertificate will throw an exception if no certificate is found.
            // Here, we are sure that the certificate is not null.            
            return certificate;
        }

        void Initialize()
        {
            this.StoreLocation = StoreLocation.LocalMachine;
            this.StoreFindType = X509FindType.FindBySubjectDistinguishedName;
            this.StoreName = StoreName.My;
        }
    }
}
