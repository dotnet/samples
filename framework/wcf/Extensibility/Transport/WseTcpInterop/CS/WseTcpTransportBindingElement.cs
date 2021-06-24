
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace Microsoft.Samples.WseTcpTransport
{
    /// <summary>
    /// Tcp Binding Element.  
    /// Used to configure and construct Tcp ChannelFactories and ChannelListeners.
    /// </summary>
    class WseTcpTransportBindingElement
        : TransportBindingElement // to signal that we're a transport
        , IPolicyExportExtension // for policy export
    {
        public WseTcpTransportBindingElement()
            : base()
        {
        }

        protected WseTcpTransportBindingElement(WseTcpTransportBindingElement other)
            : base(other)
        {
        }

        public override string Scheme
        {
            get { return "wse.tcp"; }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            return (IChannelFactory<TChannel>)(object)new WseTcpChannelFactory(this, context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            return (IChannelListener<TChannel>)(object)new WseTcpChannelListener(this, context);
        }

        // We only support IDuplexSession for our client ChannelFactories
        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return true;
            }

            return false;
        }

        // We only support IDuplexSession for our Listeners
        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return true;
            }

            return false;
        }

        public override BindingElement Clone()
        {
            return new WseTcpTransportBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // default to MTOM if no encoding is specified
            if (context.BindingParameters.Find<MessageEncodingBindingElement>() == null)
            {
                context.BindingParameters.Add(new MtomMessageEncodingBindingElement());
            }

            return base.GetProperty<T>(context);
        }

        // We expose in policy The fact that we're TCP.
        // Import is done through TcpBindingElementImporter.
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

            ICollection<XmlElement> bindingAssertions = context.GetBindingAssertions();
            XmlDocument xmlDocument = new XmlDocument();
            const string prefix = "tcp";
            const string transportAssertion = "wse.tcp";
            const string tcpPolicyNamespace = "http://sample.schemas.microsoft.com/policy/tcp";
            bindingAssertions.Add(xmlDocument.CreateElement(prefix, transportAssertion, tcpPolicyNamespace));
        }
    }
}
