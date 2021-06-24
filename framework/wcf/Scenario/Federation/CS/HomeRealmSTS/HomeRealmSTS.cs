//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Security.Permissions;
using System.ServiceModel;

namespace Microsoft.Samples.Federation
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class HomeRealmSTS : SecurityTokenService 
	{
		public HomeRealmSTS() : 
            base(ServiceConstants.StsName,
                 FederationUtilities.GetX509TokenFromCert(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.CertDistinguishedName),
                 FederationUtilities.GetX509TokenFromCert(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.TargetDistinguishedName))
		{
		}

        static double GetPurchaseLimit()
        {
            // give every authenticated caller the configured purchase limit
            return ServiceConstants.PurchaseLimit;
        }

        /// <summary>
        /// Overrides the GetIssuedClaims from the SecurityTokenService Base Class
        /// to return a valid user claim and purchase limit claim with the appropriate purchase limit 
        /// for the user
        /// </summary>
        protected override Collection<SamlAttribute> GetIssuedClaims(RequestSecurityToken requestSecurityToken)
        {
            string caller = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            double purchaseLimit = GetPurchaseLimit();

            // Create Name and PurchaseLimit claims
            Collection<SamlAttribute> samlAttributes = new Collection<SamlAttribute>();
            samlAttributes.Add(new SamlAttribute(Claim.CreateNameClaim(caller)));
            samlAttributes.Add(new SamlAttribute(new Claim ( Constants.PurchaseLimitClaim, purchaseLimit.ToString(), Rights.PossessProperty)));
            return samlAttributes;
        }

        /// <summary>
        /// This method adds the audience uri restriction condition to the SAML assetion.
        /// </summary>
        /// <param name="samlConditions">The saml condition collection where the audience uri restriction condition will be added.</param>
        public override void AddAudienceRestrictionCondition(SamlConditions samlConditions)
        {
            samlConditions.Conditions.Add(new SamlAudienceRestrictionCondition(new Uri[] { new Uri(Constants.BookStoreSTSAudienceUri) }));
        }
	}
}

