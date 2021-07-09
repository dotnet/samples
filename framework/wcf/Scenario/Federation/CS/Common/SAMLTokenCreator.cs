//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;


namespace Microsoft.Samples.Federation
{
	public sealed class SamlTokenCreator
	{
    	#region CreateSamlToken()
        /// <summary>
        /// Creates a SAML Token with the input parameters
        /// </summary>
        /// <param name="stsName">Name of the STS issuing the SAML Token</param>
        /// <param name="proofToken">Associated Proof Token</param>
        /// <param name="issuerToken">Associated Issuer Token</param>
        /// <param name="proofKeyEncryptionToken">Token to encrypt the proof key with</param>
        /// <param name="samlConditions">The Saml Conditions to be used in the construction of the SAML Token</param>
        /// <param name="samlAttributes">The Saml Attributes to be used in the construction of the SAML Token</param>
        /// <returns>A SAML Token</returns>
        public static SamlSecurityToken CreateSamlToken(string stsName,
                                                        BinarySecretSecurityToken proofToken,
                                                        SecurityToken issuerToken,
                                                        SecurityToken proofKeyEncryptionToken,
                                                        SamlConditions samlConditions,
                                                        IEnumerable<SamlAttribute> samlAttributes)
		{
            // Create a security token reference to the issuer certificate 
            SecurityKeyIdentifierClause skic = issuerToken.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>();
            SecurityKeyIdentifier issuerKeyIdentifier = new SecurityKeyIdentifier(skic);

            // Create an encrypted key clause containing the encrypted proof key
            byte[] wrappedKey = proofKeyEncryptionToken.SecurityKeys[0].EncryptKey(SecurityAlgorithms.RsaOaepKeyWrap, proofToken.GetKeyBytes());
            SecurityKeyIdentifierClause encryptingTokenClause = proofKeyEncryptionToken.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>();
            EncryptedKeyIdentifierClause encryptedKeyClause = new EncryptedKeyIdentifierClause(wrappedKey, SecurityAlgorithms.RsaOaepKeyWrap, new SecurityKeyIdentifier(encryptingTokenClause) );
            SecurityKeyIdentifier proofKeyIdentifier = new SecurityKeyIdentifier(encryptedKeyClause);

            // Create a comfirmationMethod for HolderOfKey
			List<string> confirmationMethods = new List<string>(1);
			confirmationMethods.Add(SamlConstants.HolderOfKey);

            // Create a SamlSubject with proof key and confirmation method from above
			SamlSubject samlSubject = new SamlSubject(null,
													  null,
													  null,
													  confirmationMethods,
													  null,
                                                      proofKeyIdentifier);

            // Create a SamlAttributeStatement from the passed in SamlAttribute collection and the SamlSubject from above
			SamlAttributeStatement samlAttributeStatement = new SamlAttributeStatement(samlSubject, samlAttributes);

            // Put the SamlAttributeStatement into a list of SamlStatements
			List<SamlStatement> samlSubjectStatements = new List<SamlStatement>();
			samlSubjectStatements.Add(samlAttributeStatement);

            // Create a SigningCredentials instance from the key associated with the issuerToken.
            SigningCredentials signingCredentials = new SigningCredentials(issuerToken.SecurityKeys[0],
                                                                           SecurityAlgorithms.RsaSha1Signature,
                                                                           SecurityAlgorithms.Sha1Digest,
                                                                           issuerKeyIdentifier);

            // Create a SamlAssertion from the list of SamlStatements created above and the passed in
            // SamlConditions.
			SamlAssertion samlAssertion = new SamlAssertion("_" + Guid.NewGuid().ToString(),
							                                stsName,
							                                DateTime.UtcNow,
							                                samlConditions,
							                                new SamlAdvice(),
							                                samlSubjectStatements
						                                	);
            // Set the SigningCredentials for the SamlAssertion
			samlAssertion.SigningCredentials = signingCredentials;

            // Create a SamlSecurityToken from the SamlAssertion and return it
			return new SamlSecurityToken(samlAssertion);
		}

		#endregion

        private SamlTokenCreator() { }
	}
}

