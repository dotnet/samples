//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Samples.Discovery
{

    class ReceivedCompactSignatureHeader : CompactSignatureHeader
    {
        const string bodyXPath = "//s:Envelope/s:Body";

        int headerIndex;
        MessageCachedIndexes blockIds;
        XmlDocument messageXml;
        XPathNavigator nav;
        XmlNamespaceManager nsMgr;
        string envelopePrefix;
        
        ReceivedCompactSignatureHeader(
            Message message,
            int headerIndex,
            ProtocolSettings discoveryInfo)
            : base(message, discoveryInfo)
        {
            this.headerIndex = headerIndex;
            
            // Buffer the message, so we can read the headers and the body and verify the compact signature
            this.messageXml = new XmlDocument();
            this.messageXml.LoadXml(this.Message.ToString());
            this.envelopePrefix = this.messageXml.DocumentElement.Prefix;

            nsMgr = new XmlNamespaceManager(this.messageXml.NameTable);
            nsMgr.AddNamespace("s", this.messageXml.DocumentElement.NamespaceURI);

            // Move to s:Envelope
            nav = this.messageXml.CreateNavigator();
            nav.MoveToFirstChild();

            // Only immediate children of the security header, 
            // top-level SOAP header blocks (/s:Envelope/s:Header/*), 
            // and the full SOAP Body (/s:Envelope/s:Body) can be referenced in the compact signature.
            this.blockIds = new MessageCachedIndexes();
            this.CacheAllHeaderIds();
            this.CacheBodyId();
        }
      
        public static void VerifyMessage(
            Message innerMessage, 
            ProtocolSettings discoveryInfo,
            ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings)
        {
            int headerIndex = innerMessage.Headers.FindHeader(ProtocolStrings.SecurityHeaderName, discoveryInfo.DiscoveryNamespace);
            if (headerIndex < 0)
            {
                // A security header is not present, so we can't verify the validity of the message.
                throw new CompactSignatureSecurityException("The received message doesn't contain a Security header");
            }

            ReceivedCompactSignatureHeader compactSignature = new ReceivedCompactSignatureHeader(innerMessage, headerIndex, discoveryInfo);
            compactSignature.Process(receivedCertificatesStoreSettings);
        }
                
        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            // Noop: the header is never written back
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            // Nop: the header is never written back
        }

        void Process(ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings)
        {
            // The attributes should contain the following: Scheme, KeyId, Refs, Sig, [PrefixList]
            string schemeUri;
            string keyId;
            string refs;
            string sig;
            string inclusivePrefixList;
            this.GetCompactSignatureAttributes(out schemeUri, out keyId, out refs, out sig, out inclusivePrefixList);
            this.CheckCompactSignatureAttributes(schemeUri, keyId, sig, refs, inclusivePrefixList);

            // Look for a certificate that matches the KeyId in the compact signature header
            X509Certificate2 certificate = CertificateHelper.GetCertificateByThumbprint(
                receivedCertificatesStoreSettings.StoreName,
                receivedCertificatesStoreSettings.StoreLocation,
                Utility.ToHexString(keyId));
            // Construct a ds:SignedInfo, then compute and verify signature
            this.VerifySignature(refs, sig, inclusivePrefixList, certificate);
        }
        
        void CheckCompactSignatureAttributes(string schemeUri, string keyId, string sig, string refs, string inclusivePrefixList)
        {
            if (String.IsNullOrEmpty(keyId))
            {
                throw new ArgumentNullException("keyId");
            }

            if (String.IsNullOrEmpty(refs))
            {
                throw new ArgumentNullException("refs");
            }

            if (String.IsNullOrEmpty(schemeUri))
            {
                throw new ArgumentNullException("scheme");
            }

            if (String.IsNullOrEmpty(sig))
            {
                throw new ArgumentNullException("Sig");
            }

            if (!String.IsNullOrEmpty(inclusivePrefixList) && !this.DiscoveryInfo.SupportsInclusivePrefixes)
            {
                throw new ArgumentException("Inclusive Prefixes are not supported in this discovery version.");
            }

            if (String.Compare(schemeUri, this.DiscoveryInfo.SchemeUri, true) != 0)
            {
                throw new ArgumentException("Scheme namespace doesn't have the correct value");
            }
        }

        void VerifySignature(string refs, string sig, string inclusivePrefixes, X509Certificate2 certificate)
        {
            int noHeaders = this.Message.Headers.Count;
            string[] references = refs.Split(ProtocolStrings.WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);

            using (SignatureProcessor signer = new SignatureProcessor(certificate, this.DiscoveryInfo))
            {
                if (!String.IsNullOrEmpty(inclusivePrefixes))
                {
                    signer.InclusivePrefixes = inclusivePrefixes.Split(
                        ProtocolStrings.WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);
                }

                XmlDictionaryWriter writer = signer.TakeUtf8Writer();
                bool bodyReferenced = false;
                foreach (string s in references)
                {
                    // Look for the element that has this ID - body or one of the headers
                    XPathNavigator nav;
                    XmlDictionaryReader reader;
                    if (!bodyReferenced && this.TryGetBodyBlock(s, out nav))
                    {
                        signer.AddReference(s, nav, writer);
                    }
                    else if (this.TryGetHeaderBlockWithId(s, out reader))
                    {
                        signer.AddReference(s, reader, writer);
                        reader.Close();
                    }
                    else
                    {
                        throw new CompactSignatureSecurityException(
                            string.Format(CultureInfo.CurrentCulture, "Reference with id {0} doesn't exist", s));
                    }
                }

                signer.VerifySignature(sig);
            }
        }

        void GetCompactSignatureAttributes(out string schemeUri, out string keyId, out string refs, out string sig, out string inclusivePrefixesList)
        {
            schemeUri = String.Empty;
            keyId = String.Empty;
            refs = String.Empty;
            sig = String.Empty;
            inclusivePrefixesList = null;

            XmlDictionaryReader reader = this.Message.Headers.GetReaderAtHeader(this.headerIndex);
            // reader.MoveToStartElement();
            if (reader.IsEmptyElement)
            {
                throw new CompactSignatureSecurityException("Security header doesn't contain a compact signature.");
            }

            // Move on d:Sig
            reader.ReadStartElement();
            reader.MoveToStartElement();

            while (true)
            {
                if (this.IsCompactSignatureElement(reader))
                {
                    // The d:Sig element was found
                    break;
                }

                // Skip over the children of the current node, 
                // because Sig must be a direct child of the Security header.
                reader.Skip();

                if (this.IsEndSecurityElement(reader))
                {
                    // The end element of Security was reached.
                    throw new CompactSignatureSecurityException("The security header doesn't contain a compact signature");
                }
            }

            bool schemeFound = false;
            bool keyIdFound = false;
            bool refsFound = false;
            bool sigFound = false;
            bool inclusivePrefixesFound = false;

            int attributeCount = reader.AttributeCount;
            if (attributeCount != 0)
            {
                reader.MoveToFirstAttribute();
                for (int i = 0; i < attributeCount; i++)
                {
                    string attribLocalName = reader.LocalName;
                    string value = string.Empty;
                    while (reader.ReadAttributeValue())
                    {
                        if (value.Length == 0)
                        {
                            value = reader.Value;
                        }
                        else
                        {
                            value += reader.Value;
                        }
                    }

                    if (!schemeFound && String.Compare(attribLocalName, ProtocolStrings.CompactSignatureSchemeAttribute, true) == 0)
                    {
                        // Scheme
                        schemeUri = value;
                        schemeFound = true;
                    }
                    else if (!keyIdFound && String.Compare(attribLocalName, ProtocolStrings.CompactSignatureKeyIdAttribute, true) == 0)
                    {
                        // KeyId
                        keyId = value;
                        keyIdFound = true;
                    }
                    else if (!refsFound && String.Compare(attribLocalName, ProtocolStrings.CompactSignatureRefsAttribute, true) == 0)
                    {
                        // Refs
                        refs = value;
                        refsFound = true;
                    }
                    else if (!sigFound && String.Compare(attribLocalName, ProtocolStrings.CompactSignatureElementName, true) == 0)
                    {
                        // Sig
                        sig = value;
                        sigFound = true;
                    }
                    else if (!inclusivePrefixesFound && String.Compare(attribLocalName, ProtocolStrings.PrefixListAttribute, true) == 0)
                    {
                        // PrefixList
                        inclusivePrefixesList = value;
                        inclusivePrefixesFound = true;
                    }

                    reader.MoveToNextAttribute();
                }
            }

            reader.Close();
        }

        XPathNavigator CreateBodyNavigator()
        {
            // select s:Body
            return nav.SelectSingleNode(bodyXPath, nsMgr);
        }

        void CacheAllHeaderIds()
        {
            int crtHeaderPosition = 0;
            XmlDictionaryReader reader = this.Message.Headers.GetReaderAtHeader(0);
            while (crtHeaderPosition < this.Message.Headers.Count)
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.MoveToContent();
                }

                reader.MoveToStartElement();

                if (crtHeaderPosition != this.headerIndex)
                {
                    // Look if the header has an d:Id attribute. 
                    // The headers's children are not of interest, because 
                    // only top-level SOAP header blocks (/s:Envelope/s:Header/*) can be referenced.
                    string idValue = reader.GetAttribute(ProtocolStrings.IdAttributeName, this.DiscoveryInfo.DiscoveryNamespace);
                    if (!String.IsNullOrEmpty(idValue))
                    {
                        this.blockIds.AddHeader(idValue, crtHeaderPosition);
                    }
                }

                reader.Skip();
                crtHeaderPosition++;
            }

            reader.Close();
        }

        void CacheBodyId()
        {
            XPathNavigator nav = this.CreateBodyNavigator();
            string idValue = nav.GetAttribute(ProtocolStrings.IdAttributeName, this.DiscoveryInfo.DiscoveryNamespace);
            if (!String.IsNullOrEmpty(idValue))
            {
                this.blockIds.BodyId = idValue;
            }
        }

        bool TryGetBodyBlock(string id, out XPathNavigator bodyNavigator)
        {
            bodyNavigator = null;
            if (id == this.blockIds.BodyId)
            {
                // The referenced block is the message body.
                bodyNavigator = this.CreateBodyNavigator();
                return true;
            }

            return false;
        }

        bool TryGetHeaderBlockWithId(string id, out XmlDictionaryReader reader)
        {
            reader = null;
            int index = this.blockIds.GetHeaderIndex(id);
            if (index >= 0)
            {
                // The referenced block id one of the headers.
                reader = this.Message.Headers.GetReaderAtHeader(index);
                return true;
            }

            return false;
        }

        class MessageCachedIndexes
        {
            Dictionary<string, int> headerIndexes;

            public MessageCachedIndexes()
            {
                this.BodyId = null;
                this.headerIndexes = new Dictionary<string, int>();
            }

            public string BodyId { get; set; }

            public int GetHeaderIndex(string id)
            {
                return GetValueFromDictionary(id, this.headerIndexes);
            }

            public void AddHeader(string id, int index)
            {
                AddToDictionary(id, index, this.headerIndexes);
            }

            static void AddToDictionary(string id, int index, Dictionary<string, int> dictionary)
            {
                Utility.IfNullThrowNullArgumentException(dictionary, "dictionary");
                if (String.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException("id");
                }

                dictionary.Add(id, index);
            }

            static int GetValueFromDictionary(string id, Dictionary<string, int> dictionary)
            {
                if (String.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException("id");
                }

                if (dictionary != null && dictionary.ContainsKey(id))
                {
                    return dictionary[id];
                }

                return -1;
            }
        }
    }
}
