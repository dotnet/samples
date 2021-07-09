
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.IdentityModel.Selectors;
using System.ServiceModel.Description;


namespace Microsoft.Samples.TokenProvider
{
    public class MyUserNameClientCredentials : ClientCredentials
    {
        public MyUserNameClientCredentials()
            : base()
        {
        }

        protected override ClientCredentials CloneCore()
        {
            return new MyUserNameClientCredentials();
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            // return custom security token manager
            return new MyUserNameSecurityTokenManager(this);
        }
    }
}

