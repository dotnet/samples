//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.Discovery
{

    /// <summary>
    /// Stores the details of the store where the received certificates are stored.
    /// When a signed message is received, a certificate is looked up in this store.
    /// </summary>
    public class ReceivedCertificatesStoreSettings
    {
        public ReceivedCertificatesStoreSettings()
        {
            this.StoreLocation = StoreLocation.LocalMachine;
            this.StoreName = StoreName.My;
        }

        internal ReceivedCertificatesStoreSettings(ReceivedCertificatesStoreSettings receivedCertificateStoreSettings)
        {
            Utility.IfNullThrowNullArgumentException(receivedCertificateStoreSettings, "receivedCertificateStoreSettings");
            this.StoreLocation = receivedCertificateStoreSettings.StoreLocation;
            this.StoreName = receivedCertificateStoreSettings.StoreName;
        }

        public StoreLocation StoreLocation { get; set; }

        public StoreName StoreName { get; set; }
    }
}
