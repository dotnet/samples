//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{

    public class UdpSecureDiscoveryEndpoint : UdpDiscoveryEndpoint
    {
        CompactSignatureSecurityBindingElement secureBindingElement;

        public UdpSecureDiscoveryEndpoint(SigningCertificateSettings signingStoreSettings)
            : base()
        {
            this.Intitialize(base.DiscoveryVersion, signingStoreSettings);
        }

        public UdpSecureDiscoveryEndpoint(DiscoveryVersion discoveryVersion, SigningCertificateSettings signingStoreSettings) 
            : base(discoveryVersion)
        {
            this.Intitialize(discoveryVersion, signingStoreSettings);
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

        void Intitialize(DiscoveryVersion discoveryVersion, SigningCertificateSettings signingStoreSettings)
        {
            Utility.IfNullThrowNullArgumentException(signingStoreSettings, "signingStoreSettings");

            this.secureBindingElement = new CompactSignatureSecurityBindingElement(
                discoveryVersion,
                signingStoreSettings);

            Binding binding = base.Binding;
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
    }
}
