//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------


using System.IdentityModel.Selectors;

using System.Security.Permissions;

using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;

namespace Microsoft.Samples.DurableIssuedTokenProvider
{
    public class DurableIssuedTokenClientCredentials : ClientCredentials
    {
        IssuedTokenCache cache;

        public DurableIssuedTokenClientCredentials()
            : base()
        {
        }

        DurableIssuedTokenClientCredentials(DurableIssuedTokenClientCredentials other)
            : base(other)
        {
            this.cache = other.cache;
        }

        public IssuedTokenCache IssuedTokenCache
        {
            get
            {
                return this.cache;
            }
            set
            {
                this.cache = value;
            }
        }

        protected override ClientCredentials CloneCore()
        {
            return new DurableIssuedTokenClientCredentials(this);
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            return new DurableIssuedTokenClientCredentialsTokenManager((DurableIssuedTokenClientCredentials)this.Clone());
        }

        class DurableIssuedTokenClientCredentialsTokenManager : ClientCredentialsSecurityTokenManager
        {
            IssuedTokenCache cache;

            public DurableIssuedTokenClientCredentialsTokenManager(DurableIssuedTokenClientCredentials creds)
                : base(creds)
            {
                this.cache = creds.IssuedTokenCache;
            }

            public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
            {
                if (IsIssuedSecurityTokenRequirement(tokenRequirement))
                {
                    return new DurableIssuedSecurityTokenProvider((IssuedSecurityTokenProvider)base.CreateSecurityTokenProvider(tokenRequirement), this.cache);
                }
                else
                {
                    return base.CreateSecurityTokenProvider(tokenRequirement);
                }
            }
        }
    }
}

