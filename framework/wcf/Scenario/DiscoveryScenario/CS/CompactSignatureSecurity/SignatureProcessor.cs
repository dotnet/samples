//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Samples.Discovery
{

    // We are using the following scheme of the signatures:
    // canonicalization: C14N
    // digest algorithm: SHA-1
    // signature algorithm: RSA
    class SignatureProcessor : IDisposable
    {
        readonly string SHA1SignatureName;
        static string signatureScheleton;
        static string referenceWithPrefixes;
        static string referenceNoPrefixes;
        
        ProtocolSettings discoveryInfo;
        string keyId;
        string signatureText;
        byte[] signatureValue;
        List<ReferenceEntry> references;
        HashStream hashStream;
        XmlDictionaryWriter utf8Writer;

        public SignatureProcessor(X509Certificate2 certificate, ProtocolSettings discoveryInfo)
        {
            this.Certificate = certificate;
            this.discoveryInfo = discoveryInfo;
            this.SHA1SignatureName = ProtocolStrings.SignatureAlgorithmSHA1Uri;
            this.references = new List<ReferenceEntry>();
        }

        static string ExpandedSignatureScheleton
        {
            get
            {
                if (String.IsNullOrEmpty(SignatureProcessor.signatureScheleton))
                {
                    StringBuilder sb = new StringBuilder();

                    Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignedInfoElementName, false, false);
                    Utility.AppendXmlsnAttribute(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignatureNamespace, true);
                    Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.CanonicalizationMethodElement, false, false);
                    Utility.AppendAttribute(sb, ProtocolStrings.AlgorithmAttributeName, ProtocolStrings.CanonicalizationAlgorithmUri, true);
                    Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.CanonicalizationMethodElement, true, true);
                    Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignatureMethodElement, false, false);
                    Utility.AppendAttribute(sb, ProtocolStrings.AlgorithmAttributeName, ProtocolStrings.SignatureAlgorithmSHA1Uri, true);
                    Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignatureMethodElement, true, true);

                    // Add the reference place holder
                    sb.Append("{0}");
                    Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignedInfoElementName, true, true);

                    SignatureProcessor.signatureScheleton = sb.ToString();
                }
                return SignatureProcessor.signatureScheleton;
            }
        }

        static string ExtendedSignatureReferenceWithPrefixes
        {
            get
            {
                if (String.IsNullOrEmpty(SignatureProcessor.referenceWithPrefixes))
                {
                    SignatureProcessor.referenceWithPrefixes = SignatureProcessor.BuildExpandedSignatureReferenceScheleton(true);
                }
                return SignatureProcessor.referenceWithPrefixes;
            }
        }

        static string ExtendedSignatureReferenceNoPrefixes
        {
            get
            {
                if (String.IsNullOrEmpty(SignatureProcessor.referenceNoPrefixes))
                {
                    SignatureProcessor.referenceNoPrefixes = SignatureProcessor.BuildExpandedSignatureReferenceScheleton(false);
                }
                return SignatureProcessor.referenceNoPrefixes;
            }
        }

        public string[] InclusivePrefixes { get; set; }

        public X509Certificate2 Certificate { get; private set; }

        public string KeyId
        {
            get
            {
                if (String.IsNullOrEmpty(this.keyId))
                {
                    this.keyId = CertificateHelper.GetKeyId(this.Certificate);
                }

                return this.keyId;
            }
        }
        
        public string SignatureText
        {
            get
            {
                if (this.signatureText == null)
                {
                    this.signatureText = Convert.ToBase64String(this.signatureValue);
                }

                return this.signatureText;
            }
        }

        string InclusivePrefixesList
        {
            get
            {
                if (this.InclusivePrefixes == null || this.InclusivePrefixes.Length == 0)
                {
                    return null;
                }

                StringBuilder sb = new StringBuilder();
                int prefixesLength = this.InclusivePrefixes.Length;
                for (int i = 0; i < prefixesLength - 1; i++)
                {
                    sb.Append(this.InclusivePrefixes[i]);
                    sb.Append(" ");
                }

                sb.Append(this.InclusivePrefixes[prefixesLength - 1]);
                return sb.ToString();
            }
        }

        public void SetSignatureValue(string s)
        {
            this.signatureText = s;
            this.signatureValue = Convert.FromBase64String(s);
        }
        
        // <summary>
        // Write the compact signature.
        // </summary>
        // <param name="writer">The XmlDictionaryWriter where the signature is written.</param>
        public void WriteTo(XmlDictionaryWriter writer)
        {
            // write ds:Sig
            writer.WriteStartElement(this.discoveryInfo.DiscoveryPrefix, ProtocolStrings.CompactSignatureElementName, this.discoveryInfo.DiscoveryNamespace);

            // write Scheme attribute
            writer.WriteAttributeString(ProtocolStrings.CompactSignatureSchemeAttribute, this.discoveryInfo.SchemeUri);

            // write KeyId
            string keyId = this.KeyId;
            if (!String.IsNullOrEmpty(keyId))
            {
                writer.WriteAttributeString(ProtocolStrings.CompactSignatureKeyIdAttribute, Utility.ToBase64String(keyId));
            }

            // add refs based on the values from SignedInfo
            string references = this.GetReferencesString();
            writer.WriteAttributeString(ProtocolStrings.CompactSignatureRefsAttribute, references);

            // write Sig
            writer.WriteAttributeString(ProtocolStrings.CompactSignatureElementName, this.SignatureText);

            // write PrefixList
            if (this.InclusivePrefixes != null)
            {
                writer.WriteAttributeString(ProtocolStrings.PrefixListAttribute, this.InclusivePrefixesList);
            }

            // close - </d:Sig ...>
            writer.WriteEndElement();
        }

        public void ComputeSignature()
        {
            AsymmetricAlgorithm privateKey;
            SignatureDescription description;
            HashAlgorithm hash;
            this.ComputeHash(out privateKey, out description, out hash);

            AsymmetricSignatureFormatter formatter = description.CreateFormatter(privateKey);
            byte[] signatureValue = formatter.CreateSignature(hash);
            this.signatureValue = signatureValue;
        }

        public void VerifySignature(string providedSignature)
        {
            this.SetSignatureValue(providedSignature);

            AsymmetricAlgorithm privateKey;
            SignatureDescription description;
            HashAlgorithm hash;
            this.ComputeHash(out privateKey, out description, out hash);

            AsymmetricSignatureDeformatter deformatter = description.CreateDeformatter(privateKey);
            if (!deformatter.VerifySignature(hash, this.signatureValue))
            {
                throw new CompactSignatureSecurityException("Signature verification failed");
            }
        }
        
        public void AddReference(string id, byte[] digest)
        {
            this.references.Add(new ReferenceEntry(id, digest));
        }
               
        public void Dispose()
        {
            if (this.hashStream != null)
            {
                this.hashStream.Close();
                this.hashStream = null;
            }
        }

        public void AddReference(string headerId, XPathNavigator navigator, XmlDictionaryWriter writer)
        {
            HashStream hashStream = this.TakeHashStream();
            writer.StartCanonicalization(hashStream, false, this.InclusivePrefixes);

            // The reader must be positioned on the start element of the header / body we want to canonicalize
            writer.WriteNode(navigator, false);
            writer.EndCanonicalization();
            writer.Flush();

            // Add a reference for this block
            this.AddReference(headerId, hashStream.FlushHashAndGetValue());
        }

        public void AddReference(string headerId, XmlDictionaryReader reader, XmlDictionaryWriter writer)
        {
            HashStream hashStream = this.TakeHashStream();
            writer.StartCanonicalization(hashStream, false, this.InclusivePrefixes);

            // The reader must be positioned on the start element of the header / body we want to canonicalize
            writer.WriteNode(reader, false);
            writer.EndCanonicalization();
            writer.Flush();

            // Add a reference for this block
            this.AddReference(headerId, hashStream.FlushHashAndGetValue());
        }

        public void AddReference(
            MessageHeaders headers, 
            int i,
            XmlDictionaryWriter writer, 
            string headerId, 
            bool idInserted)
        {
            HashStream hashStream = this.TakeHashStream();

            writer.StartCanonicalization(hashStream, false, this.InclusivePrefixes);
            headers.WriteStartHeader(i, writer);
            if (idInserted)
            {
                writer.WriteAttributeString(this.discoveryInfo.DiscoveryPrefix, ProtocolStrings.IdAttributeName, this.discoveryInfo.DiscoveryNamespace, headerId);
            }

            headers.WriteHeaderContents(i, writer);
            writer.WriteEndElement();
            writer.EndCanonicalization();
            writer.Flush();

            // Add a pre-digested reference for this header
            this.AddReference(headerId, hashStream.FlushHashAndGetValue());
        }

        public HashStream TakeHashStream(HashAlgorithm hash)
        {
            if (this.hashStream == null)
            {
                this.hashStream = new HashStream(hash);
            }
            else
            {
                this.hashStream.Reset(hash);
            }

            return this.hashStream;
        }

        public HashStream TakeHashStream()
        {
            return this.TakeHashStream(new SHA1CryptoServiceProvider());
        }

        public XmlDictionaryWriter TakeUtf8Writer()
        {
            if (this.utf8Writer == null)
            {
                this.utf8Writer = XmlDictionaryWriter.CreateTextWriter(Stream.Null, Encoding.UTF8, false);
            }
            else
            {
                ((IXmlTextWriterInitializer)this.utf8Writer).SetOutput(Stream.Null, Encoding.UTF8, false);
            }

            return this.utf8Writer;
        }

        void ComputeHash(out AsymmetricAlgorithm privateKey, out SignatureDescription description, out HashAlgorithm hash)
        {
            privateKey = this.Certificate.PrivateKey;
            description = CryptoConfig.CreateFromName(this.SHA1SignatureName) as SignatureDescription;
            if (description == null)
            {
                throw new CompactSignatureSecurityException(string.Format(
                     CultureInfo.CurrentCulture,
                     "Error creating SignatureDescription from the signature name {0}",
                     this.SHA1SignatureName));
            }

            hash = description.CreateDigest();
            if (hash == null)
            {
                throw new CompactSignatureSecurityException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Error creating HashAlgorithm from the signature name {0}",
                    this.SHA1SignatureName));
            }

            HashStream hashStream = this.TakeHashStream(hash);

            // Create the references
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(this.InclusivePrefixesList))
            {
                for (int i = 0; i < this.references.Count; i++)
                {
                    sb.Append(string.Format(SignatureProcessor.ExtendedSignatureReferenceWithPrefixes,
                                this.references[i].Id,
                                this.InclusivePrefixesList,
                                Convert.ToBase64String(this.references[i].Digest)));
                }
            }
            else
            {
                for (int i = 0; i < this.references.Count; i++)
                {
                    sb.Append(string.Format(SignatureProcessor.ExtendedSignatureReferenceNoPrefixes,
                                this.references[i].Id,
                                Convert.ToBase64String(this.references[i].Digest)));
                }
            }

            string expandedSignature = string.Format(
                ExpandedSignatureScheleton,
                // Replace the references
                sb.ToString());

            byte[] bytes = Encoding.UTF8.GetBytes(expandedSignature);
            hashStream.Write(bytes, 0, bytes.Length);
            hashStream.FlushHash();
        }

        static string BuildExpandedSignatureReferenceScheleton(bool addInclusivePrefixes)
        {
            StringBuilder sb = new StringBuilder();
            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.ReferenceElementName, false, false);
            // {0} = this.references[i].Id
            Utility.AppendAttribute(sb, ProtocolStrings.ReferenceURIElementName, "#{0}", true);
            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignedInfoTransforms, false, true);
            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignedInfoTransform, false, false);
            Utility.AppendAttribute(sb, ProtocolStrings.AlgorithmAttributeName, ProtocolStrings.CanonicalizationAlgorithmUri, true);

            if (addInclusivePrefixes)
            {
                Utility.AppendLocalName(sb, ProtocolStrings.InclusiveNamespacesPrefix, ProtocolStrings.InclusiveNamespacesElementName, false, false);
                Utility.AppendXmlsnAttribute(sb, "ec", ProtocolStrings.CanonicalizationAlgorithmUri, false);
                // {1} = inclusivePrefixesList
                Utility.AppendAttribute(sb, ProtocolStrings.PrefixListAttribute, "{1}", true);
                Utility.AppendLocalName(sb, ProtocolStrings.InclusiveNamespacesPrefix, ProtocolStrings.InclusiveNamespacesElementName, true, true);
            }

            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignedInfoTransform, true, true);
            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.SignedInfoTransforms, true, true);
            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.DigestMethodElement, false, false);
            Utility.AppendAttribute(sb, ProtocolStrings.AlgorithmAttributeName, ProtocolStrings.DigestAlgorithmUri, true);
            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.DigestMethodElement, true, true);
            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.DigestValueElement, false, true);

            // Write the digest value
            // {1/2} = Convert.ToBase64String(this.references[i].Digest)
            sb.Append(addInclusivePrefixes ? "{2}" : "1");

            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.DigestValueElement, true, true);
            Utility.AppendLocalName(sb, ProtocolStrings.SignaturePrefix, ProtocolStrings.ReferenceElementName, true, true);

            return sb.ToString();
        }

        string GetReferencesString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.references.Count - 1; i++)
            {
                sb.Append(this.references[i].Id);
                sb.Append(" ");
            }

            sb.Append(this.references[this.references.Count - 1].Id);
            return sb.ToString();
        }

        struct ReferenceEntry
        {
            public string Id { get; set; }

            public byte[] Digest { get; set; }

            public ReferenceEntry(string referenceId, byte[] digestValue)
                : this()
            {
                this.Id = referenceId;
                this.Digest = digestValue;
            }
        }
    }
}
