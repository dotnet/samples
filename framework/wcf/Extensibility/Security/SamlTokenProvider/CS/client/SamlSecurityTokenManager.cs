//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace Microsoft.Samples.SamlTokenProvider
{
    /// <summary>
    /// Custom SecurityTokenManager that creates a custom SecurityTokenProvider
    /// </summary>
    public class SamlSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        /// <summary>
        /// The SamlClientCredentials that created us
        /// </summary>
        SamlClientCredentials samlClientCredentials;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="samlClientCredentials">The creating SamlClientCredentials instance</param>
        public SamlSecurityTokenManager(SamlClientCredentials samlClientCredentials)
            : base(samlClientCredentials)
        {
            // Store the creating client credentials
            this.samlClientCredentials = samlClientCredentials;
        }

        /// <summary>
        /// Creates the custom SecurityTokenProvider when SAML tokens are specified in the tokenRequirement
        /// </summary>
        /// <param name="tokenRequirement">A SecurityTokenRequirement  </param>
        /// <returns>The appropriate SecurityTokenProvider</returns>
        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            // If token requirement matches SAML token return the custom SAML token provider            
            if (tokenRequirement.TokenType == SecurityTokenTypes.Saml ||
                tokenRequirement.TokenType == "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1")
            {
                // Retrieve the SAML assertion and proof token from the client credentials
                SamlAssertion assertion = this.samlClientCredentials.Assertion;
                SecurityToken prooftoken = this.samlClientCredentials.ProofToken;

                // If either the assertion of proof token is null...
                if (assertion == null || prooftoken == null)
                {
                    // ...get the SecurityBindingElement and then the specified algorithm suite
                    SecurityBindingElement sbe = null;
                    SecurityAlgorithmSuite sas = null;

                    if (tokenRequirement.TryGetProperty<SecurityBindingElement>("http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement/SecurityBindingElement", out sbe))
                    {
                        sas = sbe.DefaultAlgorithmSuite;
                    }

                    // If the token requirement is for a SymmetricKey based token..
                    if (tokenRequirement.KeyType == SecurityKeyType.SymmetricKey)
                    {
                        // Create a symmetric proof token
                        prooftoken = SamlUtilities.CreateSymmetricProofToken(tokenRequirement.KeySize);
                        // and a corresponding assertion based on the claims specified in the client credentials
                        assertion = SamlUtilities.CreateSymmetricKeyBasedAssertion(this.samlClientCredentials.Claims,
                                                                               new X509SecurityToken(samlClientCredentials.ClientCertificate.Certificate),
                                                                               new X509SecurityToken(samlClientCredentials.ServiceCertificate.DefaultCertificate),
                                                                               (BinarySecretSecurityToken)prooftoken,
                                                                               sas);
                    }
                    // otherwise...
                    else
                    {
                        // Create an asymmetric proof token
                        prooftoken = SamlUtilities.CreateAsymmetricProofToken();
                        // and a corresponding assertion based on the claims specified in the client credentials
                        assertion = SamlUtilities.CreateAsymmetricKeyBasedAssertion(this.samlClientCredentials.Claims, prooftoken, sas);
                    }

                }

                // Create a SamlSecurityTokenProvider based on the assertion and proof token
                return new SamlSecurityTokenProvider(assertion, prooftoken);
            }
            // otherwise use base implementation
            else
            {
                return base.CreateSecurityTokenProvider(tokenRequirement);
            }                
        }
    }
}

