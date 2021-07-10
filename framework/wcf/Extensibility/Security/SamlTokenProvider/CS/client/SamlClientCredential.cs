//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.IdentityModel.Claims;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

using System.ServiceModel.Description;

namespace Microsoft.Samples.SamlTokenProvider
{
    /// <summary>
    /// Custom client credentials class that allows a SAML assertion and associated proof token to be specified
    /// </summary>
    public class SamlClientCredentials : ClientCredentials
    {
        /// <summary>
        /// A ClaimSet containing the claims that will be put into the SAML assertion
        /// </summary>
        ClaimSet claims;

        /// <summary>
        /// The SAML assertion
        /// </summary>
        SamlAssertion assertion;

        /// <summary>
        /// The proof token associated with the SAML assertion
        /// </summary>
        SecurityToken proofToken;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public SamlClientCredentials()
            : base()
        {
            // Set SupportInteractive to false to suppress Cardspace UI
            base.SupportInteractive = false;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">The SamlClientCredentials to create a copy of</param>
        protected SamlClientCredentials(SamlClientCredentials other) : base ( other )
        {
            // Just do reference copy given sample nature
            this.assertion = other.assertion;            
            this.claims = other.claims;
            this.proofToken = other.proofToken;            
        }

        /// <summary>
        /// Property allowing the SAML assertion to be specified and retrieved
        /// </summary>
        public SamlAssertion Assertion { get { return assertion; } set { assertion = value; } }

        /// <summary>
        /// Property allowing the proof token to be specified and retrieved
        /// </summary>
        public SecurityToken ProofToken { get { return proofToken; } set { proofToken = value; } }

        /// <summary>
        /// Property allowing the ClaimSet to be specified and retrieved
        /// </summary>
        public ClaimSet Claims { get { return claims; } set { claims = value; } }

        protected override ClientCredentials CloneCore()
        {
            return new SamlClientCredentials(this);            
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            // return custom security token manager
            return new SamlSecurityTokenManager(this);
        }
    }
}

