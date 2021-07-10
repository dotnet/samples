//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.IdentityModel.Tokens;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace Microsoft.Samples.WS2007FederationHttpBinding
{
    // This class is specific to the version WS-Trust13.
    public class RequestSecurityTokenWSTrust13 : RequestSecurityTokenBase
    {
        // Private members
        private string keyType;     // Tracks the type of the proof key (if any).
        private string requestType; // Tracks the request type (for example, Issue, Renew, Cancel).        
        private SecurityToken requestorEntropy;
        private SecurityToken proofKey;

        // Constructors.
        /// <summary>
        /// Default constructor
        /// </summary>
        public RequestSecurityTokenWSTrust13() : this(String.Empty, String.Empty, String.Empty, 0, Constants.Trust13.KeyTypes.Symmetric, null, null, null)
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="context">The value of the wst:RequestSecurityToken/@Context attribute</param>
        /// <param name="tokenType">The content of the wst:RequestSecurityToken/wst:TokenType element</param>
        /// <param name="requestType"></param>
        /// <param name="keySize">The content of the wst:RequestSecurityToken/wst:KeySize element</param>
        /// <param name="keyType"></param>
        /// <param name="proofKey"></param>
        /// <param name="entropy">A SecurityToken representing entropy provided by the requester in the wst:RequestSecurityToken/wst:Entropy element</param>
        /// <param name="claimTypeRequirements"></param>
        /// <param name="appliesTo">The content of the wst:RequestSecurityToken/wst:KeySize element</param>
        public RequestSecurityTokenWSTrust13(string context, string tokenType, string requestType, int keySize, string keyType , SecurityToken proofKey, SecurityToken entropy, EndpointAddress appliesTo) : base ( context, tokenType,keySize, appliesTo )
        {
            this.keyType = keyType;
            this.proofKey = proofKey;
            this.requestType = requestType;
            this.requestorEntropy = entropy;
        }

        // Public properties.
        public string RequestType 
        { 
            get { return requestType; }
            set { requestType = value; }
        }

        public string KeyType
        {
            get { return keyType; }
            set { keyType = value; }
        }

        public SecurityToken ProofKey
        {
            get { return proofKey; }
            set { proofKey = value; }
        }

        /// <summary>
        /// The SecurityToken that represents entropy provided by the requester.
        /// Null if the requester did not provide entropy.
        /// </summary>
        public SecurityToken RequestorEntropy
        {
            get { return requestorEntropy; }
            set { requestorEntropy = value; }
        }

        // public methods
        public bool IsProofKeyAsymmetric()
        {
            return Constants.Trust13.KeyTypes.Public == keyType;
        }

        /// <summary>
        /// Reads a wst:RequestSecurityToken element, its attributes and children and 
        /// creates a RequestSecurityToken instance with the appropriate values.
        /// </summary>
        /// <param name="xr">An XmlReader positioned on wst:RequestSecurityToken</param>
        /// <returns>A RequestSecurityToken instance, initialized with the data read from the XmlReader</returns>
        public static RequestSecurityTokenWSTrust13 CreateFrom(XmlReader xr)
        {
            return ProcessRequestSecurityTokenElement(xr);        
        }

        // Methods of BodyWriter.
        /// <summary>
        /// Writes out an XML representation of the instance.        
        /// </summary>
        /// <param name="writer">The writer to be used to write out the XML content</param>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            // Write out the wst:RequestSecurityToken start tag.
            writer.WriteStartElement(Constants.Trust13.Elements.RequestSecurityToken, Constants.Trust13.NamespaceUri);

            // If there is a non-null, non-empty tokenType...
            if (this.TokenType != null && this.TokenType.Length > 0)
            {
                // Write out the wst:TokenType start tag.
                writer.WriteStartElement(Constants.Trust13.Elements.TokenType, Constants.Trust13.NamespaceUri);
                // Write out the tokenType string.
                writer.WriteString(this.TokenType);
                writer.WriteEndElement(); // wst:TokenType
            }

            // If there is a non-null, non-empty requestType...
            if (this.requestType != null && this.requestType.Length > 0)
            {
                // Write out the wst:RequestType start tag.
                writer.WriteStartElement(Constants.Trust13.Elements.RequestType, Constants.Trust13.NamespaceUri);
                // Write out the requestType string.
                writer.WriteString(this.requestType);
                writer.WriteEndElement(); // wst:RequestType
            }

            // If there is a non-null appliesTo.
            if (this.AppliesTo != null)
            {
                // Write out the wsp:AppliesTo start tag.
                writer.WriteStartElement(Constants.Policy.Elements.AppliesTo, Constants.Policy.NamespaceUri);
                // Write the appliesTo in WS-Addressing 1.0 format.
                this.AppliesTo.WriteTo(AddressingVersion.WSAddressing10, writer);
                writer.WriteEndElement(); // wsp:AppliesTo
            }

            if (this.requestorEntropy!=null)
            {
                writer.WriteStartElement(Constants.Trust13.Elements.Entropy, Constants.Trust13.NamespaceUri);
                BinarySecretSecurityToken bsst = this.requestorEntropy as BinarySecretSecurityToken;
                if (bsst!=null)
                {
                    writer.WriteStartElement(Constants.Trust13.Elements.BinarySecret, Constants.Trust13.NamespaceUri);
                    byte[] key = bsst.GetKeyBytes();
                    writer.WriteBase64(key, 0, key.Length);
                    writer.WriteEndElement(); // wst:BinarySecret
                }
                writer.WriteEndElement(); // wst:Entropy
            }

            if (this.keyType != null && this.keyType.Length > 0)
            {
                writer.WriteStartElement(Constants.Trust13.Elements.KeyType, Constants.Trust13.NamespaceUri);
                writer.WriteString(this.keyType);
                writer.WriteEndElement(); // wst:KeyType
            }

            if (this.KeySize> 0)
            {
                writer.WriteStartElement(Constants.Trust13.Elements.KeySize, Constants.Trust13.NamespaceUri);
                writer.WriteValue(this.KeySize );
                writer.WriteEndElement(); // wst:KeySize
            }

            writer.WriteEndElement(); // wst:RequestSecurityToken
        }


        // Private methods.

        /// <summary>
        /// Reads the wst:RequestSecurityToken element.
        /// </summary>
        /// <param name="xr">An XmlReader, positioned on the start tag of wst:RequestSecurityToken</param>
        /// <returns>A RequestSecurityToken instance, initialized with the data read from the XmlReader</returns>
        private static RequestSecurityTokenWSTrust13 ProcessRequestSecurityTokenElement(XmlReader xr)
        {
            // If the provided XmlReader is null, throw an exception.
            if (xr == null)
                throw new ArgumentNullException("xr");

            // If the wst:RequestSecurityToken element is empty, then throw an exception.
            if (xr.IsEmptyElement)
                throw new ArgumentException("wst:RequestSecurityToken element was empty. Unable to create RequestSecurityToken object");

            // Store the initial depth so this function can be exited when corresponding end-tag is reached.
            int initialDepth = xr.Depth;

            // Extract the @Context attribute value.                           
            string context = xr.GetAttribute(Constants.Trust13.Attributes.Context, String.Empty);
            
            string tokenType = String.Empty;
            string requestType = String.Empty;
            int keySize = 0;
            string keyType = Constants.Trust13.KeyTypes.Symmetric;            
            EndpointAddress appliesTo = null;
            SecurityToken entropy = null;
            SecurityToken proofKey = null;

            // Enter a read loop...
            while (xr.Read())
            {                
                // Process element start tags.
                if (XmlNodeType.Element == xr.NodeType)
                {
                    // Process WS-Trust13 elements.
                    if (Constants.Trust13.NamespaceUri == xr.NamespaceURI)
                    {
                        if (Constants.Trust13.Elements.RequestType == xr.LocalName &&
                             !xr.IsEmptyElement)
                        {
                            xr.Read();
                            requestType = xr.ReadContentAsString();
                        }
                        else if (Constants.Trust13.Elements.TokenType == xr.LocalName &&
                                 !xr.IsEmptyElement)
                        {
                            xr.Read();
                            tokenType = xr.ReadContentAsString();
                        }
                        else if (Constants.Trust13.Elements.KeySize == xr.LocalName &&
                                 !xr.IsEmptyElement)
                        {
                            xr.Read();
                            keySize = xr.ReadContentAsInt();                            
                        }
                        else if (Constants.Trust13.Elements.KeyType == xr.LocalName &&
                                 !xr.IsEmptyElement)
                        {
                            xr.Read();
                            keyType = xr.ReadContentAsString();
                        }
                        else if (Constants.Trust13.Elements.Entropy == xr.LocalName &&
                            !xr.IsEmptyElement)
                        {
                            entropy = ProcessEntropyElement(xr);
                        }
                        else
                        {
                            Console.WriteLine("Not processing element: {0}:{1}", xr.NamespaceURI, xr.LocalName);
                        }
                    }
                    // Process WS-Policy elements.
                    else if (Constants.Policy.NamespaceUri == xr.NamespaceURI)
                    {
                        if (Constants.Policy.Elements.AppliesTo == xr.LocalName &&
                            !xr.IsEmptyElement)
                        {
                            appliesTo = ProcessAppliesToElement(xr);
                        }
                        else
                        {
                            Console.WriteLine("Not processing element: {0}:{1}", xr.NamespaceURI, xr.LocalName);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not processing element: {0}:{1}", xr.NamespaceURI, xr.LocalName);
                    }
                }

                // Look for the end-tag that corresponds to the start-tag the reader was positioned 
                // on when the method was called.
                if (Constants.Trust13.Elements.RequestSecurityToken == xr.LocalName &&
                    Constants.Trust13.NamespaceUri == xr.NamespaceURI &&
                    xr.Depth == initialDepth &&
                    XmlNodeType.EndElement == xr.NodeType )
                    break;
            }

            // Construct a new RequestSecurityToken based on the values read and return it.
            return new RequestSecurityTokenWSTrust13(context, tokenType, requestType, keySize, keyType, proofKey, entropy, appliesTo);
        }

        /// <summary>
        /// Reads a wst:Entropy element and constructs a SecurityToken.
        /// Assumes that the provided entropy is never more than 1Kb in size.
        /// </summary>
        /// <param name="xr">An XmlReader positioned on the start tag of wst:Entropy</param>
        /// <returns>A SecurityToken containing the entropy value</returns>
        private static SecurityToken ProcessEntropyElement(XmlReader xr)
        {
            // If the provided XmlReader is null, throw an exception.
            if (xr == null)
                throw new ArgumentNullException("xr");

            // If the wst:Entropy element is empty, then throw an exception.
            if (xr.IsEmptyElement)
                throw new ArgumentException("wst:Entropy element was empty. Unable to create SecurityToken object");

            // Store the initial depth so this function can be exited when the corresponding end-tag is reached.            
            int initialDepth = xr.Depth;

            // Set the return value to null.
            SecurityToken st = null;

            // Enter a read loop...
            while (xr.Read())
            {
                // Look for a non-empty wst:BinarySecret element.
                if (Constants.Trust13.Elements.BinarySecret == xr.LocalName &&
                         Constants.Trust13.NamespaceUri == xr.NamespaceURI &&
                         !xr.IsEmptyElement &&
                         XmlNodeType.Element == xr.NodeType)
                {
                    // Allocate a 1024 byte buffer for the entropy.
                    byte[] temp = new byte[1024];

                    // Move the reader to the content of wst:BinarySecret element...
                    xr.Read();

                    // ...and read that content as base64. Store the actual number of bytes.                    
                    int nBytes = xr.ReadContentAsBase64(temp, 0, temp.Length);

                    // Allocate a new array of the correct size to hold the provided entropy.
                    byte[] entropy = new byte[nBytes];

                    // Copy the entropy from the temporary array into the new array.
                    for (int i = 0; i < nBytes; i++)
                        entropy[i] = temp[i];

                    // Create new BinarySecretSecurityToken from the provided entropy.
                    st = new BinarySecretSecurityToken(entropy);
                }

                // Look for the end-tag that corresponds to the start-tag the reader was positioned 
                // on when the method was called. When it is found, break out of the read loop.
                if (Constants.Trust13.Elements.Entropy == xr.LocalName &&
                    Constants.Trust13.NamespaceUri == xr.NamespaceURI &&
                    xr.Depth == initialDepth &&
                    XmlNodeType.EndElement == xr.NodeType)
                    break;
            }

            return st;
        }

        /// <summary>
        /// Reads a wsp:AppliesTo element.
        /// </summary>
        /// <param name="xr">An XmlReader positioned on the start tag of wsp:AppliesTo</param>
        /// <returns>An EndpointAddress</returns>
        private static EndpointAddress ProcessAppliesToElement(XmlReader xr)
        {
            // If the provided XmlReader is null, throw an exception.
            if (xr == null)
                throw new ArgumentNullException("xr");

            // If the wsp:AppliesTo element is empty, then throw an exception.
            if (xr.IsEmptyElement)
                throw new ArgumentException("wsp:AppliesTo element was empty. Unable to create EndpointAddress object");

            // Store the initial depth so this function can be exited when the corresponding end-tag is reached.
            int initialDepth = xr.Depth;

            // Set the return value to null.
            EndpointAddress ea = null;

            // Enter a read loop...
            while (xr.Read())
            {
                // Look for a WS-Addressing 1.0 Endpoint Reference...
                if (Constants.Addressing.Elements.EndpointReference == xr.LocalName &&
                         Constants.Addressing.NamespaceUri == xr.NamespaceURI &&
                         !xr.IsEmptyElement &&
                         XmlNodeType.Element == xr.NodeType)
                {
                    // Create a DataContractSerializer for an EndpointAddress10.
                    DataContractSerializer dcs = new DataContractSerializer(typeof(EndpointAddress10));
                    // Read the EndpointAddress10 from the DataContractSerializer.
                    EndpointAddress10 ea10 = (EndpointAddress10)dcs.ReadObject(xr, false);
                    // Convert the EndpointAddress10 into an EndpointAddress.
                    ea = ea10.ToEndpointAddress();
                }
                // Look for a WS-Addressing 2004/08 Endpoint Reference...
                else if (Constants.Addressing.Elements.EndpointReference == xr.LocalName &&
                         Constants.Addressing.NamespaceUriAugust2004 == xr.NamespaceURI &&
                         !xr.IsEmptyElement &&
                         XmlNodeType.Element == xr.NodeType)
                {
                    // Create a DataContractSerializer for an EndpointAddressAugust2004.
                    DataContractSerializer dcs = new DataContractSerializer(typeof(EndpointAddressAugust2004));
                    // Read the EndpointAddressAugust2004 from the DataContractSerializer.
                    EndpointAddressAugust2004 eaAugust2004 = (EndpointAddressAugust2004)dcs.ReadObject(xr, false);
                    // Convert the EndpointAddressAugust2004 into an EndpointAddress.
                    ea = eaAugust2004.ToEndpointAddress();
                }

                // Look for the end-tag that corresponds to the start-tag the reader was positioned 
                // on when the method was called. When it is found, break out of the read loop.
                if (Constants.Policy.Elements.AppliesTo == xr.LocalName &&
                    Constants.Policy.NamespaceUri == xr.NamespaceURI &&
                    xr.Depth == initialDepth &&
                    XmlNodeType.EndElement == xr.NodeType)
                    break;
            }

            // Return the EndpointAddress.
            return ea;
        }
    }
}

