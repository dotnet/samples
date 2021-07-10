//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.Discovery
{

    abstract class CompactSignatureHeader : MessageHeader
    {
        public CompactSignatureHeader(Message message, ProtocolSettings discoveryInfo)
        {
            Utility.IfNullThrowNullArgumentException(message, "message");
            Utility.IfNullThrowNullArgumentException(discoveryInfo, "discoveryInfo");

            this.Message = message;
            this.DiscoveryInfo = discoveryInfo;
        }

        public Message Message
        {
            get;
            private set;
        }

        public override string Name
        {
            get { return ProtocolStrings.SecurityHeaderName; }
        }

        public string Prefix
        {
            get { return DiscoveryInfo.DiscoveryPrefix; }
        }

        public override string Namespace
        {
            get { return this.DiscoveryInfo.DiscoveryNamespace; }
        }

        protected ProtocolSettings DiscoveryInfo { get; set; }

        protected string HeaderNamespace 
        {
            get { return this.DiscoveryInfo.DiscoveryNamespace; }
        }

        public bool IsEndSecurityElement(XmlDictionaryReader reader)
        {
            return reader.NodeType == XmlNodeType.EndElement &&
                reader.LocalName == ProtocolStrings.SecurityHeaderName &&
                reader.NamespaceURI == this.DiscoveryInfo.DiscoveryNamespace;
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            base.OnWriteStartHeader(writer, messageVersion);
        }

        protected bool IsCompactSignatureElement(XmlDictionaryReader reader)
        {
            return reader.LocalName == ProtocolStrings.CompactSignatureElementName &&
                reader.NamespaceURI == this.DiscoveryInfo.DiscoveryNamespace;
        }

        protected bool IsSecurityElement(MessageHeaderInfo header)
        {
            return header.Name == ProtocolStrings.SecurityHeaderName &&
                header.Namespace == this.DiscoveryInfo.DiscoveryNamespace;
        }
    }
}
