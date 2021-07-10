//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IdentityModel.Selectors;
using System.ServiceModel.Description;


namespace Microsoft.Samples.CustomToken
{
    /// <summary>
    /// CreditCardServiceCredentials for use with the Credit Card Token. It serves up a Custom SecurityTokenManager
    /// CreditCardServiceCredentialsSecurityTokenManager - that knows about our custom token.
    /// </summary>
    /// 
    public class CreditCardServiceCredentials : ServiceCredentials
    {
        string creditCardFile;

        public CreditCardServiceCredentials(string creditCardFile)
            : base()
        { 
            if (creditCardFile == null)
                throw new ArgumentNullException("creditCardFile");

            this.creditCardFile = creditCardFile;
        }

        public string CreditCardDataFile
        {
            get { return this.creditCardFile; }
        }

        protected override ServiceCredentials CloneCore()
        {
            return new CreditCardServiceCredentials(this.creditCardFile);
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            return new CreditCardServiceCredentialsSecurityTokenManager(this);
        }
    }
}

