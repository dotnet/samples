//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace Microsoft.Samples.SamlTokenProvider
{
	class SamlUtilities
	{
        /// <summary>
        /// Creates a SAML assertion based on a symmetric proof key
        /// </summary>
        /// <param name="claims">A ClaimSet containing the claims to be placed into the SAML assertion</param>
        /// <param name="signatureToken">An X509SecurityToken that will be used to sign the SAML assertion</param>
        /// <param name="encryptionToken">An X509SecurityToken that will be used to encrypt the proof key</param>
        /// <param name="proofToken">A BinarySecretSecurityToken containing the proof key</param>
        /// <param name="algoSuite">The algorithm suite to use when performing cryptographic operations</param>
        /// <returns>A SAML assertion containing the passed in claims and proof key, signed by the provided signature token</returns>
        public static SamlAssertion CreateSymmetricKeyBasedAssertion(ClaimSet claims, X509SecurityToken signatureToken, X509SecurityToken encryptionToken, BinarySecretSecurityToken proofToken, SecurityAlgorithmSuite algoSuite)
        {
            // Check various input parameters
            if (claims == null)
                throw new ArgumentNullException("claims");

            if (claims.Count == 0)
                throw new ArgumentException("Provided ClaimSet must contain at least one claim");

            if (proofToken == null)
                throw new ArgumentNullException("proofToken");

            if (signatureToken == null)
                throw new ArgumentNullException("signatureToken");

            if (encryptionToken == null)
                throw new ArgumentNullException("encryptionToken");

            if (proofToken == null)
                throw new ArgumentNullException("proofToken");

            if (algoSuite == null)
                throw new ArgumentNullException("algoSuite");
            
            // Get signing key and a key identifier for same
            SecurityKey signatureKey = signatureToken.SecurityKeys[0];
            SecurityKeyIdentifierClause signatureSkic = signatureToken.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>();            
            SecurityKeyIdentifier signatureKeyIdentifier  = new SecurityKeyIdentifier(signatureSkic);

            // Get encryption key and a key identifier for same
            SecurityKey encryptionKey = encryptionToken.SecurityKeys[0];
            SecurityKeyIdentifierClause encryptionSkic = encryptionToken.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>();
            SecurityKeyIdentifier encryptionKeyIdentifier = new SecurityKeyIdentifier(encryptionSkic);

            // Encrypt the proof key and create a key identifier for same
            byte[] proofKey = proofToken.GetKeyBytes();
            byte[] encryptedSecret = new byte[proofKey.Length];
            encryptedSecret = encryptionKey.EncryptKey(algoSuite.DefaultAsymmetricKeyWrapAlgorithm, proofKey);
            SecurityKeyIdentifier proofKeyIdentifier = new SecurityKeyIdentifier(new EncryptedKeyIdentifierClause(encryptedSecret, algoSuite.DefaultAsymmetricKeyWrapAlgorithm, encryptionKeyIdentifier));

            // Create the assertion
            return CreateAssertion(claims, signatureKey, signatureKeyIdentifier, proofKeyIdentifier, algoSuite);          
        }

        /// <summary>
        /// Creates a SAML assertion based on an Asymmetric proof key
        /// </summary>
        /// <param name="claims">A ClaimSet containing the claims to be placed into the SAML assertion</param>
        /// <param name="proofToken">An  RsaSecurityToken containing the proof key</param>
        /// <param name="algoSuite">The algorithm suite to use when performing cryptographic operations</param>
        /// <returns>A SAML assertion containing the passed in claims and proof key, signed by the proof key</returns>
        public static SamlAssertion CreateAsymmetricKeyBasedAssertion(ClaimSet claims, SecurityToken proofToken, SecurityAlgorithmSuite algoSuite)
        {
            // Check various input parameters
            if (claims == null)
                throw new ArgumentNullException("claims");

            if (proofToken == null)
                throw new ArgumentNullException("proofToken");

            if (claims.Count == 0)
                throw new ArgumentException("Provided ClaimSet must contain at least one claim");

            // Create key identifier for proof key
            SecurityKeyIdentifier proofKeyIdentifier = new SecurityKeyIdentifier(proofToken.CreateKeyIdentifierClause<RsaKeyIdentifierClause>());
            // Get signing key and a key identifier for same
            SecurityKey signatureKey = proofToken.SecurityKeys[0];
            SecurityKeyIdentifier signatureKeyIdentifier = proofKeyIdentifier;

            // Create the assertion
            return CreateAssertion(claims, signatureKey, signatureKeyIdentifier, proofKeyIdentifier, algoSuite);
        }
       
        /// <summary>
        /// Creates a SAML assertion based on input parameters
        /// </summary>
        /// <param name="claims">A ClaimSet containing the claims to be placed into the SAML assertion</param>
        /// <param name="signatureKey">The SecurityKey that will be used to sign the SAML assertion</param>
        /// <param name="signatureKeyIdentifier">A key identifier for the signature key</param>
        /// <param name="proofKeyIdentifier">A key identifier for the proof key</param>
        /// <param name="algoSuite">The algorithm suite to use when performing cryptographic operations</param>
        /// <returns>A SAML assertion containing the passed in claims and proof key, signed by the provided signature key</returns>
        private static SamlAssertion CreateAssertion(ClaimSet claims, SecurityKey signatureKey, SecurityKeyIdentifier signatureKeyIdentifier, SecurityKeyIdentifier proofKeyIdentifier, SecurityAlgorithmSuite algoSuite)
        {
            List<string> confirmationMethods = new List<string>(1);
            // Create a confirmationMethod for HolderOfKey                
            confirmationMethods.Add(SamlConstants.HolderOfKey);

            // Create a SamlSubject with proof key and confirmation method from above
            SamlSubject samlSubject = new SamlSubject(null,
                                                      null,
                                                      "Self",
                                                      confirmationMethods,
                                                      null,
                                                      proofKeyIdentifier);

            IList<SamlAttribute> samlAttributes = new List<SamlAttribute>();
            foreach (Claim c in claims)
            {
                if ( typeof(string) == c.Resource.GetType())
                    samlAttributes.Add(new SamlAttribute(c));
            }

            // Create a SamlAttributeStatement from the passed in SamlAttribute collection and the 
            // SamlSubject from above
            SamlAttributeStatement samlAttributeStatement = new SamlAttributeStatement(samlSubject, samlAttributes);

            // Put the SamlAttributeStatement into a list of SamlStatements
            List<SamlStatement> samlSubjectStatements = new List<SamlStatement>();
            samlSubjectStatements.Add(samlAttributeStatement);

            // Create a SigningCredentials instance from the signature key
            SigningCredentials signingCredentials = new SigningCredentials(signatureKey,
                                                                           algoSuite.DefaultAsymmetricSignatureAlgorithm,
                                                                           algoSuite.DefaultDigestAlgorithm,
                                                                           signatureKeyIdentifier);

            // Create a SamlAssertion from the list of SamlStatements created above 
            DateTime issueInstant = DateTime.UtcNow;
            
            // Create the Saml condition with allowed audience Uris
            SamlConditions conditions = new SamlConditions(issueInstant, issueInstant + new TimeSpan(10, 0, 0));
            conditions.Conditions.Add(new SamlAudienceRestrictionCondition(new Uri[] { new Uri("http://localhost:8000/servicemodelsamples/service/calc/symm"),
                                                                                       new Uri("http://localhost:8000/servicemodelsamples/service/calc/asymm") }));

            SamlAssertion samlAssertion = new SamlAssertion("_" + Guid.NewGuid().ToString(),
                                                            "Self",
                                                            issueInstant,
                                                            conditions,
                                                            new SamlAdvice(),
                                                            samlSubjectStatements
                                                            );

            // Set the SigningCredentials for the SamlAssertion
            samlAssertion.SigningCredentials = signingCredentials;

            // Return the SamlAssertion
            return samlAssertion;
        }

        /// <summary>
        /// Creates a BinarySecretSecurityToken
        /// </summary>
        /// <param name="keySize">Number of bits of key material to generate</param>
        /// <returns>A BinarySecretSecurityToken</returns>
        public static SecurityToken CreateSymmetricProofToken(int keySize)
        {
            if (keySize < 128 || keySize > 2048)
                throw new ArgumentOutOfRangeException("keySize", "must be in the range 128 to 2048");

            // Create a secret key
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] secret = new byte[keySize / 8];
            random.GetNonZeroBytes(secret);
            return new BinarySecretSecurityToken(secret);
        }

        /// <summary>
        /// Creates an RsaSecurityToken
        /// </summary>
        /// <returns>An RsaSecurityToken</returns>
        public static SecurityToken CreateAsymmetricProofToken()
        {
            return new RsaSecurityToken(RSA.Create());
        }       

        private SamlUtilities(){}
	}
}

