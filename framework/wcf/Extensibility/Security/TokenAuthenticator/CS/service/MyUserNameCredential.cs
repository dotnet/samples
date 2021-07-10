
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.IdentityModel.Selectors;
using System.ServiceModel.Description;

namespace Microsoft.Samples.TokenAuthenticator
{
    public class MyUserNameCredential : ServiceCredentials
    {

        public MyUserNameCredential()
            : base()
        {
        }

        protected override ServiceCredentials CloneCore()
        {
            return new MyUserNameCredential();
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            return new MySecurityTokenManager(this);
        }

    }
}


