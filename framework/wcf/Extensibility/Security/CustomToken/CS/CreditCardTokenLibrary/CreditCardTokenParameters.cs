//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;

namespace Microsoft.Samples.CustomToken
{
    /// <summary>
    /// CreditCardTokenParameters for use with the Credit Card Token
    /// </summary>
    public class CreditCardTokenParameters : SecurityTokenParameters
    {
        string issuer;

        public CreditCardTokenParameters() : this((string) null) { }

        public CreditCardTokenParameters(string issuer) : base() 
        {
            this.issuer = issuer;
        }

        protected CreditCardTokenParameters(CreditCardTokenParameters other)
            : base(other)
        {
            this.issuer = other.issuer;
        }

        protected override SecurityTokenParameters CloneCore()
        {
            return new CreditCardTokenParameters(this);
        }

        public string Issuer
        {
            get { return this.issuer; }
        }

        protected override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            requirement.TokenType = Constants.CreditCardTokenType;
            return; 
        }

        // A credit card token has no crypto, no windows identity and supports only client authentication
        protected override bool HasAsymmetricKey { get { return false; } }
        protected override bool SupportsClientAuthentication { get { return true; } }
        protected override bool SupportsClientWindowsIdentity { get { return false; } }
        protected override bool SupportsServerAuthentication { get { return false; } }

        protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
        {
            if (referenceStyle == SecurityTokenReferenceStyle.Internal)
            {
                return token.CreateKeyIdentifierClause<LocalIdKeyIdentifierClause>();
            }
            else
            {
                throw new NotSupportedException("External references are not supported for credit card tokens");
            }
        }
    }
}

