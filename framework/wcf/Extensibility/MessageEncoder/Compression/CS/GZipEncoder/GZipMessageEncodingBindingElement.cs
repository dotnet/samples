//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace Microsoft.Samples.GZipEncoder
{
    // This is constants for GZip message encoding policy.
    internal static class GZipMessageEncodingPolicyConstants
    {
        public const string GZipEncodingName = "GZipEncoding";
        public const string GZipEncodingNamespace = "http://schemas.microsoft.com/ws/06/2004/mspolicy/netgzip1";
        public const string GZipEncodingPrefix = "gzip";
    }

    //This is the binding element that, when plugged into a custom binding, will enable the GZip encoder
    public sealed class GZipMessageEncodingBindingElement
                        : MessageEncodingBindingElement //BindingElement
#if NET462_OR_GREATER
                        , IPolicyExportExtension
#endif
    {

        //By default, use the default text encoder as the inner encoder
        public GZipMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement()) { }

        public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement) => InnerMessageEncodingBindingElement = messageEncoderBindingElement;

        //We will use an inner binding element to store information required for the inner encoder
        public MessageEncodingBindingElement InnerMessageEncodingBindingElement { get; set; }

        //Main entry point into the encoder binding element. Called by WCF to get the factory that will create the
        //message encoder
        public override MessageEncoderFactory CreateMessageEncoderFactory() => new GZipMessageEncoderFactory(InnerMessageEncodingBindingElement.CreateMessageEncoderFactory());

        public override MessageVersion MessageVersion
        {
            get => InnerMessageEncodingBindingElement.MessageVersion;
            set => InnerMessageEncodingBindingElement.MessageVersion = value;
        }

        public override BindingElement Clone() => new GZipMessageEncodingBindingElement(this.InnerMessageEncodingBindingElement);

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return InnerMessageEncodingBindingElement.GetProperty<T>(context);
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

#if NET462_OR_GREATER
        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
        {
            if (policyContext == null)
            {
                throw new ArgumentNullException("policyContext");
            }
            XmlDocument document = new XmlDocument();
            policyContext.GetBindingAssertions().Add(document.CreateElement(
                GZipMessageEncodingPolicyConstants.GZipEncodingPrefix,
                GZipMessageEncodingPolicyConstants.GZipEncodingName,
                GZipMessageEncodingPolicyConstants.GZipEncodingNamespace));
        }
#endif
    }

#if NET462_OR_GREATER
    //This class is necessary to be able to plug in the GZip encoder binding element through
    //a configuration file
    public class GZipMessageEncodingElement : System.ServiceModel.Configuration.BindingElementExtensionElement
    {
        public GZipMessageEncodingElement()
        {
        }

        //Called by the WCF to discover the type of binding element this config section enables
        public override Type BindingElementType => typeof(GZipMessageEncodingBindingElement);

        //The only property we need to configure for our binding element is the type of
        //inner encoder to use. Here, we support text and binary.
        [ConfigurationProperty("innerMessageEncoding", DefaultValue = "textMessageEncoding")]
        public string InnerMessageEncoding
        {
            get => (string)base["innerMessageEncoding"];
            set => base["innerMessageEncoding"] = value;
        }

        //Called by the WCF to apply the configuration settings (the property above) to the binding element
        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            GZipMessageEncodingBindingElement binding = (GZipMessageEncodingBindingElement)bindingElement;
            PropertyInformationCollection propertyInfo = this.ElementInformation.Properties;
            if (propertyInfo["innerMessageEncoding"].ValueOrigin != PropertyValueOrigin.Default)
            {
                switch (this.InnerMessageEncoding)
                {
                    case "textMessageEncoding":
                        binding.InnerMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
                        break;
                    case "binaryMessageEncoding":
                        binding.InnerMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
                        break;
                }
            }
        }

        //Called by the WCF to create the binding element
        protected override BindingElement CreateBindingElement()
        {
            GZipMessageEncodingBindingElement bindingElement = new GZipMessageEncodingBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }
    }
#endif
}
