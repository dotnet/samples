//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.Discovery
{

    sealed class SendCompactSignatureHeader : CompactSignatureHeader, IDisposable
    {
        X509Certificate2 certificate;
        SignatureProcessor signer;

        public SendCompactSignatureHeader(Message message, X509Certificate2 certificate, ProtocolSettings discoveryInfo)
            : base(message, discoveryInfo)
        {
            this.certificate = certificate;
        }

        public SignedMessage SignedMessage
        {
            get { return (SignedMessage)this.Message; }
        }

        public void StartSignature()
        {
            this.signer = new SignatureProcessor(this.certificate, this.DiscoveryInfo);
        }

        public void CompleteSignature()
        {
            this.signer.ComputeSignature();
        }

        public void Dispose()
        {
            if (this.signer != null)
            {
                this.signer.Dispose();
                this.signer = null;
            }
        }

        public void ApplySecurityAndWriteHeaders(MessageHeaders headers, XmlDictionaryWriter writer, SecurityIdGenerator securityIdGenerator)
        {
            // There is no way to look through the headers attributes without changing the way 
            // Headers.WriterStartHeader / headers.writeHeadercontents writes the header
            // So i'm using a copy that I can change without worries.
            MessageHeaders copyHeaders = new MessageHeaders(headers);

            for (int i = 0; i < headers.Count; i++)
            {
                MessageHeaderInfo header = headers[i];

                // We are not supporting another d:Security header, throw if there is already one in the message
                if (this.IsSecurityElement(header))
                {
                    throw new ArgumentException("The message already contains a d:security header.");
                }

                if (this.ShouldProtectHeader(header))
                {
                    string headerId;
                    bool idInserted;
                    this.GetHeaderId(copyHeaders.GetReaderAtHeader(i), securityIdGenerator, true, out headerId, out idInserted);

                    // Add a reference for this header
                    this.signer.AddReference(headers, i, writer, headerId, idInserted);
                }
                else
                {
                    headers.WriteHeader(i, writer);
                }
            }
        }

        public void ApplyBodySecurity(XmlDictionaryWriter writer)
        {
            SignedMessage message = this.SignedMessage;
            HashStream hashStream = this.signer.TakeHashStream();
            
            message.WriteBodyToSign(hashStream, writer);
            this.signer.InclusivePrefixes = message.InclusivePrefixes;
            this.signer.AddReference(message.BodyId, hashStream.FlushHashAndGetValue());
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            // Write d:Security 
            writer.WriteStartElement(DiscoveryInfo.DiscoveryPrefix, this.Name, this.Namespace);

            if (this.signer != null)
            {
                // write <d:Sig ...>
                this.signer.WriteTo(writer);
            }

            WriteHeaderAttributes(writer, messageVersion);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            // the compact signature header doesn't have any content
        }

        void GetHeaderId(
            XmlDictionaryReader reader,
            SecurityIdGenerator securityIdGenerator,
            bool closeReader,
            out string headerId,
            out bool idInserted)
        {
            // Look if the header already has a discovery Id attribute defined
            headerId = reader.GetAttribute(ProtocolStrings.IdAttributeName, this.DiscoveryInfo.DiscoveryNamespace);
            if (closeReader)
            {
                reader.Close();
            }

            idInserted = false;
            if (String.IsNullOrEmpty(headerId))
            {
                // The header doesn't contain a d:Id, so generate one.
                headerId = securityIdGenerator.GenerateId();
                idInserted = true;
            }
        }

        bool ShouldProtectHeader(MessageHeaderInfo header)
        {
            // Only /s:Envelope/s:Header/* and /s:Envelope/s:Body can be referenced.
            // From those, we want to protect the addressing and the discovery headers
            // together with the Body.
            return header.Namespace == this.DiscoveryInfo.AddressingNamespace ||
                header.Namespace == this.DiscoveryInfo.DiscoveryNamespace;
        }
    }
}
