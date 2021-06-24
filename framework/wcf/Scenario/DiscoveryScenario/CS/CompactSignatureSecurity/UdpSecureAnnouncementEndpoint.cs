//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{

    public class UdpSecureAnnouncementEndpoint : UdpAnnouncementEndpoint
    {
        CompactSignatureSecurityBindingElement secureBindingElement;

        public UdpSecureAnnouncementEndpoint(SigningCertificateSettings signingStoreSettings)
            : base()
        {
            this.Intialize(signingStoreSettings);
        }

        public UdpSecureAnnouncementEndpoint(DiscoveryVersion discoveryVersion,  SigningCertificateSettings signingStoreSettings)
            : base(discoveryVersion)
        {
            this.Intialize(signingStoreSettings);
        }

        private void Intialize(SigningCertificateSettings signingStoreSettings)
        {
            Utility.IfNullThrowNullArgumentException(signingStoreSettings, "signingStoreSettings");

            this.secureBindingElement = new CompactSignatureSecurityBindingElement(
                base.DiscoveryVersion,
                signingStoreSettings);

            Binding binding = this.Binding;
            CustomBinding customBinding = binding as CustomBinding;
            if (customBinding == null)
            {
                customBinding = new CustomBinding(binding);
                customBinding.Elements.Insert(0, secureBindingElement);
                base.Binding = customBinding;
            }
            else
            {
                customBinding.Elements.Insert(0, secureBindingElement);
            }
        }
        
        public SigningCertificateSettings SigningCertificateStoreSettings
        {
            get { return this.secureBindingElement.SigningCertificateStoreSettings; }
            set { this.secureBindingElement.SigningCertificateStoreSettings = value; }
        }

        public ReceivedCertificatesStoreSettings ReceivedCertificatesStoreSettings
        {
            get { return this.secureBindingElement.ReceivedCertificatesStoreSettings; }
            set { this.secureBindingElement.ReceivedCertificatesStoreSettings = value; }
        }
    }
}
