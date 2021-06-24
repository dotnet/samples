//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;

using System.Xml;


namespace Microsoft.Samples.Federation
{
    /// <summary>
    /// Abstract base class for STS implementations
    /// </summary>
	public abstract class SecurityTokenService : ISecurityTokenService
	{
		string stsName; // The name of the STS. Used to populate saml:Assertion/@Issuer
        SecurityToken issuerToken; // The SecurityToken used to sign issued tokens
        SecurityToken proofKeyEncryptionToken; // The SecurityToken used to encrypt the proof key in the issued token.

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="stsName">The name of the STS. Used to populate saml:Assertion/@Issuer</param>
        /// <param name="token">The X509SecurityToken that the STS uses to sign SAML assertions</param>
        /// <param name="targetServiceName">The X509SecurityToken that is used to encrypt the proof key in the SAML token.</param>
        protected SecurityTokenService(string stsName, X509SecurityToken issuerToken, X509SecurityToken encryptionToken)
        {
            this.stsName = stsName;
            this.issuerToken = issuerToken;
            this.proofKeyEncryptionToken = encryptionToken;
        }

        /// <summary>
        /// The name of the STS.
        /// </summary>
        protected string SecurityTokenServiceName
        {
            get { return this.stsName; }
        }


        /// <summary>
        /// The SecurityToken used to sign tokens the STS issues.
        /// </summary>
        protected SecurityToken IssuerToken
        {
            get { return this.issuerToken; }
        }

        /// <summary>
        /// The SecurityToken used to encrypt the proof key in the issued token.
        /// </summary>
        protected SecurityToken ProofKeyEncryptionToken
        {
            get { return this.proofKeyEncryptionToken; }
        }

		#region Abstract methods
        

        /// <summary>
        /// abstract method for setting up claims in the SAML Token issued by the STS
        /// Should be overridden by STS implementations that derive from this base class
        /// to set up appropriate claims
        /// </summary>
        protected abstract Collection<SamlAttribute> GetIssuedClaims(RequestSecurityToken requestSecurityToken);

        #endregion

        #region Helper Methods
        /// <summary>
        /// Validate action header and discard messages with inappropriate headers
        /// </summary>
        protected static void EnsureRequestSecurityTokenAction(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Headers.Action != Constants.Trust.Actions.Issue)
                throw new InvalidOperationException(String.Format("Bad or Unsupported Action: {0}", message.Headers.Action));
        }

        /// <summary>
        /// Helper Method to Create Proof Token. Creates BinarySecretSecuryToken 
        /// with the requested number of bits of random key material
        /// </summary>
        /// <param name="keySize">keySize</param>
        /// <returns>Proof Token</returns>
        protected static BinarySecretSecurityToken CreateProofToken(int keySize)
        {
            // Create an array to store the key bytes
            byte[] key = new byte[keySize/8];
            // Create some random bytes
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            random.GetNonZeroBytes(key);
            // Create a BinarySecretSecurityToken from the random bytes and return it
            return new BinarySecretSecurityToken(key);
        }

        /// <summary>
        /// Helper Method to set up the RSTR
        /// </summary>
        /// <param name="rst">RequestSecurityToken</param>
        /// <param name="keySize">keySize</param>
        /// <param name="proofToken">proofToken</param>
        /// <param name="samlToken">The SAML Token to be issued</param>
        /// <returns>RequestSecurityTokenResponse</returns>
        protected static RequestSecurityTokenBase GetRequestSecurityTokenResponse(RequestSecurityTokenBase requestSecurityToken,
                                                                                      int keySize, 
                                                                                      SecurityToken proofToken,
                                                                                      SecurityToken samlToken,
                                                                                      byte[] senderEntropy,
                                                                                      byte[] stsEntropy)
        {
            // Create an uninitialized RequestSecurityTokenResponse object and set the various properties
            RequestSecurityTokenResponse rstr = new RequestSecurityTokenResponse();
            rstr.TokenType = Constants.SamlTokenTypeUri;
            rstr.RequestedSecurityToken = samlToken;
            rstr.RequestedUnattachedReference = samlToken.CreateKeyIdentifierClause<SamlAssertionKeyIdentifierClause>();
            rstr.RequestedAttachedReference = samlToken.CreateKeyIdentifierClause<SamlAssertionKeyIdentifierClause>();
            rstr.Context = requestSecurityToken.Context;
            rstr.KeySize = keySize;

            // If sender provided entropy then use combined entropy so set the IssuerEntropy
            if (senderEntropy != null)
            {
                rstr.IssuerEntropy = new BinarySecretSecurityToken(stsEntropy);
                rstr.ComputeKey = true;
            }
            else // Issuer entropy only...
            {
                rstr.RequestedProofToken = proofToken;
            }
            
            return rstr;
        }
        #endregion

        /// <summary>
        /// Virtual method for ProcessRequestSecurityToken
        /// Should be overridden by STS implementations that derive from this base class
        /// </summary>
        public virtual Message ProcessRequestSecurityToken(Message message)
        {
            // Check for appropriate action header
            EnsureRequestSecurityTokenAction(message);

            // Extract the MessageID from the request message
            UniqueId requestMessageID = message.Headers.MessageId;
            if (requestMessageID == null)
                throw new InvalidOperationException("The request message does not have a message ID.");

            // Get the RST from the message
            RequestSecurityToken rst = RequestSecurityToken.CreateFrom(message.GetReaderAtBodyContents());

            // Set up the claims we are going to issue
            Collection<SamlAttribute> samlAttributes = GetIssuedClaims(rst);

            // get the key size, default to 192
            int keySize = (rst.KeySize != 0) ? rst.KeySize : 192;

            // Create proof token
            // Get requester entropy, if any
            byte[] senderEntropy = null;
            SecurityToken entropyToken = rst.RequestorEntropy;
            if (entropyToken != null)
            {
                senderEntropy = ((BinarySecretSecurityToken)entropyToken).GetKeyBytes();
            }

            byte[] key = null;
            byte[] stsEntropy = null;

            // If sender provided entropy, then use combined entropy
            if (senderEntropy != null)
            {
                // Create an array to store the entropy bytes
                stsEntropy = new byte[keySize / 8];
                // Create some random bytes
                RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
                random.GetNonZeroBytes(stsEntropy);
                // Compute the combined key
                key = RequestSecurityTokenResponse.ComputeCombinedKey(senderEntropy, stsEntropy, keySize);
            }
            else // Issuer entropy only...
            {
                // Create an array to store the entropy bytes
                key = new byte[keySize / 8];
                // Create some random bytes
                RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
                random.GetNonZeroBytes(key);
            }

            // Create a BinarySecretSecurityToken to be the proof token, based on the key material
            // in key. The key is the combined key in the combined entropy case, or the issuer entropy
            // otherwise
            BinarySecretSecurityToken proofToken = new BinarySecretSecurityToken(key);

            // Create the saml condition
            SamlConditions samlConditions = new SamlConditions(DateTime.UtcNow - TimeSpan.FromMinutes(5), DateTime.UtcNow + TimeSpan.FromHours(10));
            AddAudienceRestrictionCondition(samlConditions);

            // Create a SAML token, valid for around 10 hours
            SamlSecurityToken samlToken = SamlTokenCreator.CreateSamlToken(this.stsName,
                                                                           proofToken,
                                                                           this.IssuerToken,
                                                                           this.ProofKeyEncryptionToken,
                                                                           samlConditions,
                                                                           samlAttributes);

            // Set up RSTR
            RequestSecurityTokenBase rstr = GetRequestSecurityTokenResponse(rst, keySize, proofToken, samlToken, senderEntropy, stsEntropy);
            
            // Create a message from the RSTR
            Message rstrMessage = Message.CreateMessage(message.Version, Constants.Trust.Actions.IssueReply, rstr);
            
            // Set RelatesTo of response message to MessageID of request message
            rstrMessage.Headers.RelatesTo = requestMessageID;

            // Return the create message
            return rstrMessage;
        }

        /// <summary>
        /// This method adds the audience uri restriction condition to the SAML assetion.
        /// </summary>
        /// <param name="samlConditions">The saml condition collection where the audience uri restriction condition will be added.</param>
        public abstract void AddAudienceRestrictionCondition(SamlConditions samlConditions);

    }
}

