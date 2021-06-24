//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{

    class CompactSignatureSecurityBindingElement : BindingElement
    {
        public CompactSignatureSecurityBindingElement()
            : base()
        {
        }

        public CompactSignatureSecurityBindingElement(
            DiscoveryVersion discoveryVersion,
            SigningCertificateSettings signingStoreSettings)
            : base()
        {
            this.DiscoveryVersion = discoveryVersion;
            this.SigningCertificateStoreSettings = signingStoreSettings;
            this.ReceivedCertificatesStoreSettings = new ReceivedCertificatesStoreSettings();
        }
        
        public CompactSignatureSecurityBindingElement(CompactSignatureSecurityBindingElement bindingElement) 
            : base(bindingElement) 
        {
            this.DiscoveryVersion = bindingElement.DiscoveryVersion;
            this.SigningCertificateStoreSettings = new SigningCertificateSettings(bindingElement.SigningCertificateStoreSettings);
            this.ReceivedCertificatesStoreSettings = new ReceivedCertificatesStoreSettings(bindingElement.ReceivedCertificatesStoreSettings);
        }

        public DiscoveryVersion DiscoveryVersion { get; set; }

        public SigningCertificateSettings SigningCertificateStoreSettings { get; set; }

        public ReceivedCertificatesStoreSettings ReceivedCertificatesStoreSettings { get; set; }

        public override BindingElement Clone()
        {
            return new CompactSignatureSecurityBindingElement(this); 
        }

        public override T GetProperty<T>(BindingContext context)
        {
            Utility.IfNullThrowNullArgumentException(context, "context");
            return context.GetInnerProperty<T>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            Utility.IfNullThrowNullArgumentException(context, "context");
            if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return context.CanBuildInnerChannelFactory<TChannel>();
            }

            return false;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            Utility.IfNullThrowNullArgumentException(context, "context");
            if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                CompactSignatureSecurityChannelFactory channelFactory = new CompactSignatureSecurityChannelFactory(
                    context.BuildInnerChannelFactory<IDuplexChannel>(),
                    this.DiscoveryVersion,
                    this.SigningCertificateStoreSettings.GetSigningCertificate(),
                    this.ReceivedCertificatesStoreSettings);
                return (IChannelFactory<TChannel>)(object)channelFactory;
            }

            throw new ArgumentException("Unable to build a channel factory: the CompactSignatureBindingElement supports only IDuplexChannel.");
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            Utility.IfNullThrowNullArgumentException(context, "context");
            if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return context.CanBuildInnerChannelListener<TChannel>();
            }

            return false;
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            Utility.IfNullThrowNullArgumentException(context, "context");
            if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                CompactSignatureSecurityChannelListener channelListener = new CompactSignatureSecurityChannelListener(
                    context.BuildInnerChannelListener<IDuplexChannel>(),
                    this.DiscoveryVersion,
                    this.SigningCertificateStoreSettings.GetSigningCertificate(),
                    this.ReceivedCertificatesStoreSettings);
                return (IChannelListener<TChannel>)(object)channelListener;
            }

            throw new ArgumentException("Unable to build a channel listener: the CompactSignatureBindingElement supports only IDuplexChannel.");
        }
    }
}
