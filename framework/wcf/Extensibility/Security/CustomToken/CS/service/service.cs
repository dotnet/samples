
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IdentityModel.Claims; 
using System.ServiceModel;

namespace Microsoft.Samples.CustomToken
{
    // Service class which implements the service contract.
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class EchoService : IEchoService
    {
        public string Echo()
        {
            return String.Format("Hello. You presented a {0}", GetCallerCreditCardNumber());
        }

        public void Dispose()
        {
        }

        #region claim processing methods
        bool TryGetStringClaimValue(ClaimSet claimSet, string claimType, out string claimValue)
        {
            claimValue = null;
            IEnumerable<Claim> matchingClaims = claimSet.FindClaims(claimType, Rights.PossessProperty);
            if (matchingClaims == null)
                return false;
            IEnumerator<Claim> enumerator = matchingClaims.GetEnumerator();
            enumerator.MoveNext();
            claimValue = (enumerator.Current.Resource == null) ? null : enumerator.Current.Resource.ToString();
            return true;
        }

        string GetCallerCreditCardNumber()
        {
            foreach (ClaimSet claimSet in ServiceSecurityContext.Current.AuthorizationContext.ClaimSets)
            {
                string creditCardNumber = null;
                if (TryGetStringClaimValue(claimSet, Constants.CreditCardNumberClaim, out creditCardNumber))
                {
                    string issuer;
                    if (!TryGetStringClaimValue(claimSet.Issuer, ClaimTypes.Name, out issuer))
                    {
                        issuer = "Unknown";
                    }
                    return String.Format("Credit card '{0}' issued by '{1}'", creditCardNumber, issuer);
                }
            }
            return "Credit card is not known";
        }
        #endregion
    }

}

