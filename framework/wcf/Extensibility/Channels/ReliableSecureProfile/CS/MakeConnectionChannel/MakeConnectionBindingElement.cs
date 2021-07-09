//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public sealed class MakeConnectionBindingElement : BindingElement, IMakeConnectionListenerSettings, IMakeConnectionFactorySettings
        , IPolicyExportExtension
    {
        static MessagePartSpecification bodyOnly;

        TimeSpan clientPollTimeout;
        TimeSpan serverPollTimeout;

        public MakeConnectionBindingElement()
        {
            this.clientPollTimeout = MakeConnectionConstants.Defaults.ClientPollTimeout;
            this.serverPollTimeout = MakeConnectionConstants.Defaults.ServerPollTimeout;
        }

        MakeConnectionBindingElement(MakeConnectionBindingElement other)
        {
            this.clientPollTimeout = other.clientPollTimeout;
            this.serverPollTimeout = other.serverPollTimeout;
        }

        public override BindingElement Clone()
        {
            return new MakeConnectionBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                ChannelProtectionRequirements myRequirements = this.GetProtectionRequirements();
                myRequirements.Add(context.GetInnerProperty<ChannelProtectionRequirements>() ?? new ChannelProtectionRequirements());
                return (T)(object)myRequirements;
            }

            return context.GetInnerProperty<T>();
        }

        public TimeSpan ClientPollTimeout
        {
            get
            {
                return this.clientPollTimeout;
            }
            set
            {
                TimeoutHelper.ThrowIfNegativeArgument(value);
                this.clientPollTimeout = value;
            }
        }

        public TimeSpan ServerPollTimeout
        {
            get
            {
                return this.serverPollTimeout;
            }
            set
            {
                TimeoutHelper.ThrowIfNegativeArgument(value);
                this.serverPollTimeout = value;
            }
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            MessageVersion message = context.GetInnerProperty<MessageVersion>();
            if (message.Addressing == AddressingVersion.None)
            {
                return false;
            }

            if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return context.CanBuildInnerChannelFactory<IRequestChannel>();
            }

            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return context.CanBuildInnerChannelFactory<IRequestSessionChannel>();
            }

            return false;
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            IAnonymousUriPrefixMatcher anonymousUriPrefixMatcher
                = context.GetInnerProperty<IAnonymousUriPrefixMatcher>();
            if (anonymousUriPrefixMatcher == null)
            {
                return false;
            }

            MessageVersion message = context.GetInnerProperty<MessageVersion>();
            if (message.Addressing == AddressingVersion.None)
            {
                return false;
            }

            if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return context.CanBuildInnerChannelListener<IReplyChannel>();
            }

            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return context.CanBuildInnerChannelListener<IReplySessionChannel>();
            }

            return false;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            MessageVersion message = context.GetInnerProperty<MessageVersion>();
            if (message.Addressing == AddressingVersion.None)
            {
                throw new ArgumentException(ExceptionMessages.WsmcRequiresAddressing);
            }

            if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MakeConnectionChannelFactory<IDuplexChannel, IRequestChannel>(new MakeConnectionBindingElement(this), context);
            }

            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MakeConnectionChannelFactory<IDuplexSessionChannel, IRequestSessionChannel>(new MakeConnectionBindingElement(this), context);
            }

            throw new ArgumentException(String.Format(ExceptionMessages.ChannelTypeNotSupported,(typeof(TChannel))));
        }


        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            IAnonymousUriPrefixMatcher anonymousUriPrefixMatcher 
                = context.GetInnerProperty<IAnonymousUriPrefixMatcher>();
            if (anonymousUriPrefixMatcher == null)
            {
                throw new Exception(ExceptionMessages.IAnonymousUriPrefixMatcherPropertyNotFound);
            }
            anonymousUriPrefixMatcher.Register(new Uri(MakeConnectionConstants.AnonymousUriTemplate));

            MessageVersion message = context.GetInnerProperty<MessageVersion>();
            if (message.Addressing == AddressingVersion.None)
            {
                throw new ArgumentException(ExceptionMessages.WsmcRequiresAddressing);
            }

            if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (IChannelListener<TChannel>)(object)new MakeConnectionChannelListener<IDuplexChannel, IReplyChannel>(new MakeConnectionBindingElement(this), context);
            }

            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (IChannelListener<TChannel>)(object)new MakeConnectionChannelListener<IDuplexSessionChannel, IReplySessionChannel>(new MakeConnectionBindingElement(this), context);
            }

            throw new ArgumentException(String.Format
                (ExceptionMessages.ChannelTypeNotSupported,typeof(TChannel)));
        }

        static MessagePartSpecification BodyOnly
        {
            get
            {
                if (bodyOnly == null)
                {
                    MessagePartSpecification s = new MessagePartSpecification(true);
                    s.MakeReadOnly();
                    bodyOnly = s;
                }

                return bodyOnly;
            }
        }

        static void ProtectProtocolMessage(
            ScopedMessagePartSpecification signaturePart,
            ScopedMessagePartSpecification encryptionPart,
            string action)
        {
            signaturePart.AddParts(BodyOnly, action);
            encryptionPart.AddParts(MessagePartSpecification.NoParts, action);
        }

        ChannelProtectionRequirements GetProtectionRequirements()
        {
            // Listing headers that must be signed.
            ChannelProtectionRequirements result = new ChannelProtectionRequirements();
            MessagePartSpecification signedReliabilityMessageParts = MakeConnectionUtility.GetSignedReliabilityMessageParts();
            result.IncomingSignatureParts.AddParts(signedReliabilityMessageParts);
            result.OutgoingSignatureParts.AddParts(signedReliabilityMessageParts);

            // From the Client to the Service
            ScopedMessagePartSpecification signaturePart = result.IncomingSignatureParts;
            ScopedMessagePartSpecification encryptionPart = result.IncomingEncryptionParts;
            ProtectProtocolMessage(signaturePart, encryptionPart, MakeConnectionConstants.MakeConnectionMessage.Action);

            return result;
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            if (exporter == null)
            {
                throw new ArgumentNullException("exporter");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            XmlDocument document = new XmlDocument();
            context.GetBindingAssertions().Add(document.CreateElement(
                MakeConnectionConstants.Prefix,
                MakeConnectionConstants.Policy.Assertion,
                MakeConnectionConstants.Namespace));
        }

    }
}
