//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace Microsoft.Samples.CustomToken
{
    /// <summary>
    /// CreditCardTokenProvider for use with the Credit Card Token
    /// </summary>
    class CreditCardTokenProvider : SecurityTokenProvider
    {
        CreditCardInfo creditCardInfo;
        
        public CreditCardTokenProvider(CreditCardInfo creditCardInfo) : base()
        {
            if (creditCardInfo == null)
            {
                throw new ArgumentNullException("creditCardInfo");
            }
            this.creditCardInfo = creditCardInfo;
        }

        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            SecurityToken result = new CreditCardToken(this.creditCardInfo);
            return result;
        }
    }
}


