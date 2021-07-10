//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

using System.IdentityModel.Tokens;

using System.Runtime.InteropServices;

using System.Security.Cryptography;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

using System.Xml;


namespace Microsoft.Samples.Federation
{
    /// <summary>
    /// A class that represents a RequestSecurityTokenResponse message according to February 2005 WS-Trust
    /// </summary>
    [ComVisible(false)]
    public class RequestSecurityTokenResponse : RequestSecurityTokenBase
    {
        // private members
        private SecurityToken requestedSecurityToken;
        private SecurityToken requestedProofToken;
        private SecurityToken issuerEntropy;
        private SecurityKeyIdentifierClause requestedAttachedReference;
        private SecurityKeyIdentifierClause requestedUnattachedReference;
        private bool computeKey;

        // Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public RequestSecurityTokenResponse()
            : this(String.Empty, String.Empty, 0, null, null, null, false)
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="context">The value of the wst:RequestSecurityTokenResponse/@Context attribute</param>
        /// <param name="tokenType">The content of the wst:RequestSecurityTokenResponse/wst:TokenType element</param>
        /// <param name="keySize">The content of the wst:RequestSecurityTokenResponse/wst:KeySize element </param>
        /// <param name="appliesTo">An EndpointReference that corresponds to the content of the wst:RequestSecurityTokenResponse/wsp:AppliesTo element</param>
        /// <param name="requestedSecurityToken">The requested security token</param>
        /// <param name="requestedProofToken">The proof token associated with the requested security token</param>
        /// <param name="computeKey">A boolean that specifies whether a key value must be computed</param>
        public RequestSecurityTokenResponse(string context, 
                                            string tokenType, 
                                            int keySize, 
                                            EndpointAddress appliesTo, 
                                            SecurityToken requestedSecurityToken, 
                                            SecurityToken requestedProofToken, 
                                            bool computeKey ) :
            base(context, tokenType, keySize, appliesTo) // Pass first 4 params to base class
        {
            this.requestedSecurityToken = requestedSecurityToken;
            this.requestedProofToken = requestedProofToken;
            this.computeKey = computeKey;
        }

        // public properties
        /// <summary>
        /// The requested SecurityToken
        /// </summary>
        public SecurityToken RequestedSecurityToken
        {
            get { return requestedSecurityToken; }
            set { requestedSecurityToken = value; }
        }

        /// <summary>
        /// A SecurityToken that represents the proof token associated with 
        /// the requested SecurityToken
        /// </summary>
        public SecurityToken RequestedProofToken
        {
            get { return requestedProofToken; }
            set { requestedProofToken = value; }
        }

        /// <summary>
        /// A SecurityKeyIdentifierClause that can be used to refer to the requested 
        /// SecurityToken when that token is present in messages
        /// </summary>
        public SecurityKeyIdentifierClause RequestedAttachedReference
        {
            get { return requestedAttachedReference; }
            set { requestedAttachedReference = value; }
        }

        /// <summary>
        /// A SecurityKeyIdentifierClause that can be used to refer to the requested 
        /// SecurityToken when that token is present in messages
        /// </summary>
        public SecurityKeyIdentifierClause RequestedUnattachedReference
        {
            get { return requestedUnattachedReference; }
            set { requestedUnattachedReference = value; }
        }

        /// <summary>
        /// The SecurityToken that represents entropy provided by the issuer
        /// Null if the issuer did not provide entropy
        /// </summary>
        public SecurityToken IssuerEntropy
        {
            get { return issuerEntropy; }
            set { issuerEntropy = value; }
        }

        /// <summary>
        /// Indicates whether a key must be computed (typically from the combination of issuer and requester entropy)
        /// </summary>
        public bool ComputeKey
        {
            get { return computeKey; }
            set { computeKey = value; }
        }

        // public methods
        /// <summary>
        /// Static method that computes a combined key from issue and requester entropy using PSHA1 according to WS-Trust
        /// </summary>
        /// <param name="requestorEntropy">Entropy provided by the requester</param>
        /// <param name="issuerEntropy">Entropy provided by the issuer</param>
        /// <param name="keySize">Size of required key, in bits</param>
        /// <returns>Array of bytes that contain key material</returns>
        public static byte[] ComputeCombinedKey(byte[] requestorEntropy, byte[] issuerEntropy, int keySize)
        {
            KeyedHashAlgorithm kha = new HMACSHA1(requestorEntropy, true);

            byte[] key = new byte[keySize / 8]; // Final key
            byte[] a = issuerEntropy; // A(0)
            byte[] b = new byte[kha.HashSize / 8 + a.Length]; // Buffer for A(i) + seed

            for (int i = 0; i < key.Length;)
            {
                // Calculate A(i+1).                
                kha.Initialize();
                a = kha.ComputeHash(a);

                // Calculate A(i) + seed
                a.CopyTo(b, 0);
                issuerEntropy.CopyTo(b, a.Length);
                kha.Initialize();
                byte[] result = kha.ComputeHash(b);

                for (int j = 0; j < result.Length; j++)
                {
                    if (i < key.Length)
                        key[i++] = result[j];
                    else
                        break;
                }
            }

            return key;
        }

        // Methods of BodyWriter        
        /// <summary>
        /// Writes out an XML representation of the instance.
        /// </summary>
        /// <param name="writer">The writer to be used to write out the XML content</param>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            // Write out the wst:RequestSecurityTokenResponse start tag
            writer.WriteStartElement(Constants.Trust.Elements.RequestSecurityTokenResponse, Constants.Trust.NamespaceUri);

            // If we have a non-null, non-empty tokenType...
            if (this.TokenType != null && this.TokenType.Length > 0)
            {
                // Write out the wst:TokenType start tag
                writer.WriteStartElement(Constants.Trust.Elements.TokenType, Constants.Trust.NamespaceUri);
                // Write out the tokenType string
                writer.WriteString(this.TokenType);
                writer.WriteEndElement(); // wst:TokenType
            }

            // Create a serializer that knows how to write out security tokens
            WSSecurityTokenSerializer ser = new WSSecurityTokenSerializer();

            // If we have a requestedSecurityToken...
            if (this.requestedSecurityToken != null)
            {
                // Write out the wst:RequestedSecurityToken start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedSecurityToken, Constants.Trust.NamespaceUri);                
                // Write out the requested token using the serializer
                ser.WriteToken(writer, requestedSecurityToken);                    
                writer.WriteEndElement(); // wst:RequestedSecurityToken
            }

            // If we have a requestedAttachedReference...
            if ( this.requestedAttachedReference != null )
            {
                // Write out the wst:RequestedAttachedReference start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedAttachedReference, Constants.Trust.NamespaceUri);
                // Write out the reference using the serializer
                ser.WriteKeyIdentifierClause ( writer, this.requestedAttachedReference );
                writer.WriteEndElement(); // wst:RequestedAttachedReference
            }

            // If we have a requestedUnattachedReference...
            if ( this.requestedUnattachedReference != null )
            {
                // Write out the wst:RequestedUnattachedReference start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedUnattachedReference, Constants.Trust.NamespaceUri);
                // Write out the reference using the serializer
                ser.WriteKeyIdentifierClause ( writer, this.requestedUnattachedReference );
                writer.WriteEndElement(); // wst:RequestedAttachedReference
            }

            // If we have a non-null appliesTo
            if (this.AppliesTo != null)
            {
                // Write out the wsp:AppliesTo start tag
                writer.WriteStartElement(Constants.Policy.Elements.AppliesTo, Constants.Policy.NamespaceUri);
                // Write the appliesTo in WS-Addressing 1.0 format
                this.AppliesTo.WriteTo(AddressingVersion.WSAddressing10, writer);
                writer.WriteEndElement(); // wsp:AppliesTo
            }

            // If the requestedProofToken is non-null, then the STS is providing all the key material...
            if (this.requestedProofToken != null)
            {
                // Write the wst:RequestedProofToken start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedProofToken, Constants.Trust.NamespaceUri);
                // Write the proof token using the serializer
                ser.WriteToken(writer, requestedProofToken);
                writer.WriteEndElement(); // wst:RequestedSecurityToken
            }

            // If issuerEntropy is non-null and computeKey is true, then combined entropy is being used...
            if(this.issuerEntropy != null && this.computeKey ) 
            {
                // Write the wst:RequestedProofToken start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedProofToken, Constants.Trust.NamespaceUri);
                // Write the wst:ComputeKey start tag
                writer.WriteStartElement(Constants.Trust.Elements.ComputedKey, Constants.Trust.NamespaceUri);
                // Write the PSHA1 algorithm value
                writer.WriteValue(Constants.Trust.ComputedKeyAlgorithms.PSHA1);
                writer.WriteEndElement(); // wst:ComputedKey
                writer.WriteEndElement(); // wst:RequestedSecurityToken

                // Write the wst:Entropy start tag
                writer.WriteStartElement(Constants.Trust.Elements.Entropy, Constants.Trust.NamespaceUri);                    
                // Write the issuerEntropy out using the serializer
                ser.WriteToken(writer, this.issuerEntropy);
                writer.WriteEndElement(); // wst:Entropy
            }

            writer.WriteEndElement(); // wst:RequestSecurityTokenResponse
        }
    }
}

