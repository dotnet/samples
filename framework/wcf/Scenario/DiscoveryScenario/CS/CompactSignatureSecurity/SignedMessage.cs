//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Microsoft.Samples.Discovery
{

    class SignedMessage : Message
    {
        const string DefaultPrefix = "#default";

        readonly SendCompactSignatureHeader securityHeader;
        Message innerMessage;
        SecurityIdGenerator securityIdGenerator;
        ProtocolSettings discoveryInfo;
        BodyState state;
        string bodyId;
        bool bodyIdInserted;
        byte[] fullBodyFragment;
        int fullBodyFragmentLength;
        byte[] fullBodyBuffer;
        string[] inclusivePrefixes;
        string envelopeUri;
        string envelopePrefix;

        public SignedMessage(
            Message innerMessage,
            X509Certificate2 certificate,
            ProtocolSettings discoveryInfo)
        {
            Utility.IfNullThrowNullArgumentException(innerMessage, "innerMessage");
            this.innerMessage = innerMessage;
            this.envelopeUri = (innerMessage.Version.Envelope == EnvelopeVersion.Soap11) ?
                ProtocolStrings.SoapNamespace11Uri : ProtocolStrings.SoapNamespace12Uri;
            this.envelopePrefix = ProtocolStrings.SoapPrefix;

            this.discoveryInfo = discoveryInfo;
            this.securityIdGenerator = new SecurityIdGenerator();
            this.securityHeader = new SendCompactSignatureHeader(this, certificate, discoveryInfo);
            this.state = BodyState.Created;
        }

        public string BodyId
        {
            get
            {
                return this.bodyId;
            }
        }

        public string[] InclusivePrefixes
        {
            get
            {
                return this.inclusivePrefixes;
            }
        }

        public override bool IsEmpty
        {
            get { return this.innerMessage.IsEmpty; }
        }

        public override bool IsFault
        {
            get { return this.innerMessage.IsFault; }
        }

        public override MessageHeaders Headers
        {
            get { return this.innerMessage.Headers; }
        }

        public override MessageProperties Properties
        {
            get { return this.innerMessage.Properties; }
        }

        public override MessageVersion Version
        {
            get { return this.innerMessage.Version; }
        }

        protected Message InnerMessage
        {
            get { return this.innerMessage; }
        }

        public void WriteBodyToSign(Stream canonicalStream, XmlDictionaryWriter writer)
        {
            if (this.state != BodyState.Created)
            {
                throw new InvalidOperationException("The message has already been written, can't be written again");
            }

            this.SetBodyId();

            if (!Utility.CanCanonicalizeAndFragment(writer))
            {
                this.WriteCanonicalizedBody(canonicalStream);
            }
            else
            {
                this.WriteCanonicalizedBodyWithFragments(canonicalStream, writer);
            }

            this.state = BodyState.Signed;
        }
        
        protected override void OnWriteMessage(XmlDictionaryWriter writer)
        {
            // Build the SignedXml we will use to compute the signature
            this.securityHeader.StartSignature();

            // Write s:Envelope element
            this.InnerMessage.WriteStartEnvelope(writer);

            // Generate body id if it doesn't exist and buffer the body
            this.securityHeader.ApplyBodySecurity(writer);

            // Write s:Header
            writer.WriteStartElement(this.envelopePrefix, ProtocolStrings.HeaderHeaderName, this.envelopeUri);

            // For each header, look if it should be referenced in the compact signature;
            // If yes, add an d:Id, then reference it, and write it;
            // If not, just write it as it is.
            this.securityHeader.ApplySecurityAndWriteHeaders(this.Headers, writer, this.securityIdGenerator);

            // Compute the signature for the SignedXml
            this.securityHeader.CompleteSignature();

            // Write the security header itself
            this.securityHeader.WriteHeader(writer, this.Version);

            // End the s:Header element
            writer.WriteEndElement();

            // Write the Body.
            if (this.fullBodyFragment != null)
            {
                ((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(this.fullBodyFragment, 0, this.fullBodyFragmentLength);
            }
            else
            {
                this.OnWriteStartBody(writer);
                this.OnWriteBodyContents(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        protected override void OnClose()
        {
            this.innerMessage.Close();
            base.OnClose();
            try
            {
                this.InnerMessage.Close();
                this.securityHeader.Dispose();
            }
            finally
            {
                this.fullBodyBuffer = null;
                this.state = BodyState.Disposed;
            }
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            switch (this.state)
            {
                case BodyState.Created:
                    this.InnerMessage.WriteStartBody(writer);
                    return;
                case BodyState.Signed:
                    using (XmlDictionaryReader reader = Utility.CreateReader(this.fullBodyBuffer))
                    {
                        reader.MoveToStartElement();
                        writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        writer.WriteAttributes(reader, false);
                    }
                    return;
                default:
                    throw new ArgumentException("Can't write the message because it is in a bad or unknown state");
            }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            switch (this.state)
            {
                case BodyState.Created:
                    this.InnerMessage.WriteBodyContents(writer);
                    return;
                case BodyState.Signed:
                    using (XmlDictionaryReader reader = Utility.CreateReader(this.fullBodyBuffer))
                    {
                        reader.MoveToStartElement();
                        reader.ReadStartElement();
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            writer.WriteNode(reader, false);
                        }

                        reader.ReadEndElement();
                    }
                    return;
                default:
                    throw new ArgumentException("Can't write the message because it is in a bad or unknown state");
            }
        }

        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            this.innerMessage.WriteStartEnvelope(writer);
        }

        protected override string OnGetBodyAttribute(string localName, string ns)
        {
            return this.innerMessage.GetBodyAttribute(localName, ns);
        }

        void WriteInnerMessageWithId(XmlDictionaryWriter writer)
        {
            this.InnerMessage.WriteStartBody(writer);
            if (this.bodyIdInserted)
            {
                // Write generated body id
                writer.WriteAttributeString(this.discoveryInfo.DiscoveryPrefix, ProtocolStrings.IdAttributeName, this.discoveryInfo.DiscoveryNamespace, this.bodyId);
            }

            this.InnerMessage.WriteBodyContents(writer);
            writer.WriteEndElement();
        }

        void SetBodyId()
        {
            this.bodyId = this.InnerMessage.GetBodyAttribute(ProtocolStrings.IdAttributeName, this.discoveryInfo.DiscoveryNamespace);
            if (this.bodyId == null)
            {
                this.bodyId = this.securityIdGenerator.GenerateId();
                this.bodyIdInserted = true;
            }
        }

        byte[] BufferBodyAndGetInclusivePrefixes()
        {
            // Look at the body for inclusive prefixes and buffer it at the same time.
            using (MemoryStream bufferStream = new MemoryStream())
            {
                using (MemoryStream canonicalStream = new MemoryStream())
                {
                    using (XmlDictionaryWriter helperWriter = Utility.CreateWriter(bufferStream))
                    {
                        helperWriter.StartCanonicalization(canonicalStream, false, null);
                        this.WriteInnerMessageWithId(helperWriter);
                        helperWriter.EndCanonicalization();

                        // canonicalStream contains the canonicalized body without inclusive prefixes
                        this.inclusivePrefixes = this.GetInclusivePrefixesList(canonicalStream);
                        return bufferStream.ToArray();
                    }
                }
            }
        }

        string[] GetInclusivePrefixesList(MemoryStream canonicalStream)
        {
            byte[] nonCanonicalizedBody = canonicalStream.ToArray();
            string nonCanonicalizedBodyString = Encoding.UTF8.GetString(nonCanonicalizedBody);

            List<string> inclusivePrefixesList = new List<string>();
            XmlDocument xmlBody = new XmlDocument();
            xmlBody.LoadXml(nonCanonicalizedBodyString);

            // The default inclusive prefixes uses the Types element from the Discovery namespace;
            // if there are other inclusive prefixes we should be looking for,
            // add then here. EG: "//d1:Types || d2:MyInclusivePrefix"
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlBody.NameTable);
            nsMgr.AddNamespace("d1", this.discoveryInfo.DiscoveryNamespace);
            this.CollectInclusivePrefixes(xmlBody.SelectNodes("//d1:Types", nsMgr), inclusivePrefixesList);

            if (inclusivePrefixesList != null && inclusivePrefixesList.Count > 0)
            {
                return inclusivePrefixesList.ToArray();
            }

            return null;
        }

        void CollectInclusivePrefixes(XmlNodeList nodes, List<string> inclusivePrefixesList)
        {
            string prefix;
            foreach (XmlNode node in nodes)
            {
                string text = node.InnerText;
                if (!string.IsNullOrEmpty(text))
                {
                    string[] items = text.Split(ProtocolStrings.WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in items)
                    {
                        int prefixEnd = s.IndexOf(":");
                        prefix = (prefixEnd != -1) ? s.Substring(0, prefixEnd).Trim() : SignedMessage.DefaultPrefix;
                        if (!string.IsNullOrEmpty(prefix) && !inclusivePrefixesList.Contains(prefix))
                        {
                            inclusivePrefixesList.Add(prefix);
                        }
                    }
                }
            }
        }

        void WriteCanonicalizedBody(Stream canonicalStream)
        {
            byte[] buffer = null;
            if (this.discoveryInfo.SupportsInclusivePrefixes)
            {
                buffer = this.BufferBodyAndGetInclusivePrefixes();
            }

            using (MemoryStream bodyBufferStream = new MemoryStream())
            {
                using (XmlDictionaryWriter writer = Utility.CreateWriter(bodyBufferStream))
                {
                    writer.StartCanonicalization(canonicalStream, false, this.inclusivePrefixes);
                    this.WriteBufferOrMessageBody(writer, buffer);
                    writer.EndCanonicalization();
                    writer.Flush();

                    this.fullBodyBuffer = bodyBufferStream.ToArray();
                }
            }
        }

        void WriteCanonicalizedBodyWithFragments(Stream canonicalStream, XmlDictionaryWriter writer)
        {
            byte[] buffer = null;
            if (this.discoveryInfo.SupportsInclusivePrefixes)
            {
                buffer = this.BufferBodyAndGetInclusivePrefixes();
            }

            using (MemoryStream bodyBufferStream = new MemoryStream())
            {
                writer.StartCanonicalization(canonicalStream, false, this.inclusivePrefixes);
                IFragmentCapableXmlDictionaryWriter fragmentingWriter = (IFragmentCapableXmlDictionaryWriter)writer;
                fragmentingWriter.StartFragment(bodyBufferStream, false);
                this.WriteBufferOrMessageBody(writer, buffer);
                fragmentingWriter.EndFragment();
                writer.EndCanonicalization();
                writer.Flush();
               
                this.fullBodyFragmentLength = (int)bodyBufferStream.Length;
                this.fullBodyFragment = bodyBufferStream.ToArray();
            }
        }

        void WriteBufferOrMessageBody(XmlDictionaryWriter writer, byte[] buffer)
        {
            if (buffer != null)
            {
                Utility.WriteNodeToWriter(buffer, writer);
            }
            else
            {
                this.WriteInnerMessageWithId(writer);
            }
        }

        enum BodyState
        {
            Created,
            Signed,
            Disposed
        }
    }
}
