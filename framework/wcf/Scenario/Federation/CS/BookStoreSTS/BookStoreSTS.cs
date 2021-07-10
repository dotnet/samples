//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Permissions;
using System.ServiceModel;

namespace Microsoft.Samples.Federation
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class BookStoreSTS : SecurityTokenService
    {
        /// <summary>
        /// Sets up the BookStoreSTS by loading relevant Application Settings
        /// </summary>
        public BookStoreSTS()
            : base(ServiceConstants.SecurityTokenServiceName,
                   FederationUtilities.GetX509TokenFromCert(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.CertDistinguishedName),
                   FederationUtilities.GetX509TokenFromCert(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.TargetDistinguishedName))
        {
        }

        /// <summary>
        /// Overrides the SetUpClaims from the SecurityTokenService Base Class
        /// Checks if the caller can purchase the book specified in the RST and if so
        /// issues a purchase authorized claim
        /// </summary>
        protected override Collection<SamlAttribute> GetIssuedClaims(RequestSecurityToken requestSecurityToken)
        {
            EndpointAddress rstAppliesTo = requestSecurityToken.AppliesTo;

            if (rstAppliesTo == null)
            {
                throw new InvalidOperationException("No AppliesTo EndpointAddress in RequestSecurityToken");
            }

            string bookName = rstAppliesTo.Headers.FindHeader(Constants.BookNameHeaderName, Constants.BookNameHeaderNamespace).GetValue<string>();
            if (string.IsNullOrEmpty(bookName))
                throw new FaultException("The book name was not specified in the RequestSecurityToken");

            EnsurePurchaseLimitSufficient(bookName);

            Collection<SamlAttribute> samlAttributes = new Collection<SamlAttribute>();

            foreach (ClaimSet claimSet in ServiceSecurityContext.Current.AuthorizationContext.ClaimSets)
            {
                // Copy Name claims from the incoming credentials into the set of claims we're going to issue
                IEnumerable<Claim> nameClaims = claimSet.FindClaims(ClaimTypes.Name, Rights.PossessProperty);
                if (nameClaims != null)
                {
                    foreach (Claim nameClaim in nameClaims)
                    {
                        samlAttributes.Add(new SamlAttribute(nameClaim));
                    }
                }
            }

            // add a purchase authorized claim
            samlAttributes.Add(new SamlAttribute(new Claim(Constants.PurchaseAuthorizedClaim, bookName, Rights.PossessProperty)));
            return samlAttributes;
        }

        /// <summary>
        /// This method adds the audience uri restriction condition to the SAML assetion.
        /// </summary>
        /// <param name="samlConditions">The saml condition collection where the audience uri restriction condition will be added.</param>
        public override void AddAudienceRestrictionCondition(SamlConditions samlConditions)
        {
            samlConditions.Conditions.Add(new SamlAudienceRestrictionCondition(new Uri[] { new Uri(Constants.BookStoreServiceAudienceUri) }));
        }

        #region Helper Methods
        /// <summary>
        /// Wrapper for the Application level check performed at the BookStoreSTS for 
        /// the existence of required purchase limit 
        /// </summary>
        private static void EnsurePurchaseLimitSufficient(string bookName)
        {
            if (!CheckIfPurchaseLimitMet(bookName))
            {
                throw new FaultException(String.Format("Purchase limit not sufficient to purchase '{0}'", bookName));
            }
        }

        /// <summary>
        /// Helper method to get book price from the Books Database
        /// </summary>
        /// <param name="bookID">ID of the book intended for purchase</param>
        /// <returns>Price of the book with the given ID</returns>
        private static double GetBookPrice(string bookName)
        {
            using (StreamReader myStreamReader = new StreamReader(ServiceConstants.BookDB))
            {
                string line = "";
                while ((line = myStreamReader.ReadLine()) != null)
                {
                    string[] splitEntry = line.Split('#');
                    if (splitEntry[1].Trim().Equals(bookName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return Double.Parse(splitEntry[3]);
                    }
                }
                // invalid bookName - throw
                throw new FaultException("Invalid Book Name " + bookName);
            }
        }

        /// <summary>
        /// Application level check for claims at the BookStoreSTS
        /// </summary>
        /// <param name="bookID">ID of the book intended for purchase</param>
        /// <returns>True on success. False on failure.</returns>
        private static bool CheckIfPurchaseLimitMet(string bookID)
        {
            // Extract the AuthorizationContext from the ServiceSecurityContext
            AuthorizationContext authContext = OperationContext.Current.ServiceSecurityContext.AuthorizationContext;

            // If there are no Claims in the AuthorizationContext, return false
            // The issued token used to authenticate should contain claims 
            if (authContext.ClaimSets == null)
                return false;

            // If there is more than two ClaimSets in the AuthorizationContext, return false
            // The issued token used to authenticate should only contain two sets of claims.
            if (authContext.ClaimSets.Count != 2)
                return false;

            List<ClaimSet> claimsets = new List<ClaimSet>(authContext.ClaimSets);
            ClaimSet myClaimSet = claimsets.Find((Predicate<ClaimSet>)delegate(ClaimSet target)
            {
                X509CertificateClaimSet certClaimSet = target.Issuer as X509CertificateClaimSet;
                return certClaimSet != null && certClaimSet.X509Certificate.Subject == "CN=HomeRealmSTS.com";
            });

            // Is the ClaimSet was NOT issued by the HomeRealmSTS then return false
            // The BookStoreSTS only accepts requests where the client authenticated using a token
            // issued by the HomeRealmSTS.
            if (!IssuedByHomeRealmSTS(myClaimSet))
                return false;

            // Find all the PurchaseLimit claims
            IEnumerable<Claim> purchaseLimitClaims = myClaimSet.FindClaims(Constants.PurchaseLimitClaim, Rights.PossessProperty);

            // If there are no PurchaseLimit claims, return false
            // The HomeRealmSTS issues tokens containing PurchaseLimit claims for all authorized requests.
            if (purchaseLimitClaims == null)
                return false;

            // Get the price of the book being purchased...
            double bookPrice = GetBookPrice(bookID);

            // Iterate through the PurchaseLimit claims and verify that the Resource value is 
            // greater than or equal to the price of the book being purchased
            foreach (Claim c in purchaseLimitClaims)
            {
                double purchaseLimit = Double.Parse(c.Resource.ToString());
                if (purchaseLimit >= bookPrice)
                    return true;
            }

            // If no PurchaseLimit claim had a resource value that was greater than or equal
            // to the price of the book being purchased, return false
            return false;
        }


        /// <summary>
        /// Helper function to check if SAML Token was issued by HomeRealmSTS
        /// </summary>
        /// <returns>True on success. False on failure.</returns>
        private static bool IssuedByHomeRealmSTS(ClaimSet myClaimSet)
        {
            // Extract the issuer ClaimSet
            ClaimSet issuerClaimSet = myClaimSet.Issuer;

            // If the Issuer is null, return false.
            if (issuerClaimSet == null)
                return false;

            // Find all the Thumbprint claims in the issuer ClaimSet
            IEnumerable<Claim> issuerClaims = issuerClaimSet.FindClaims(ClaimTypes.Thumbprint, null);

            // If there are no Thumbprint claims, return false;
            if (issuerClaims == null)
                return false;

            // Get the enumerator for the set of Thumbprint claims...                        
            IEnumerator<Claim> issuerClaimsEnum = issuerClaims.GetEnumerator();

            // ...and set issuerClaim to the first such Claim
            Claim issuerClaim = null;
            if (issuerClaimsEnum.MoveNext())
                issuerClaim = issuerClaimsEnum.Current;

            // If there was no Thumbprint claim, return false;
            if (issuerClaim == null)
                return false;

            // If, despite the above checks, the returned claim is not a Thumbprint claim, return false
            if (issuerClaim.ClaimType != ClaimTypes.Thumbprint)
                return false;

            // If the returned claim doesn't contain a Resource, return false
            if (issuerClaim.Resource == null)
                return false;

            // Extract the thmubprint data from the claim
            byte[] issuerThumbprint = (byte[])issuerClaim.Resource;

            // Extract the thumbprint for the HomeRealmSTS.com certificate
            byte[] certThumbprint = FederationUtilities.GetCertificateThumbprint(ServiceConstants.CertStoreName,
                                                                     ServiceConstants.CertStoreLocation,
                                                                     ServiceConstants.IssuerDistinguishedName);

            // If the lengths of the two thumbprints are different, return false
            if (issuerThumbprint.Length != certThumbprint.Length)
                return false;

            // Check the individual bytes of the two thumbprints for equality...
            for (int i = 0; i < issuerThumbprint.Length; i++)
            {
                //... if any byte in the thumbprint from the claim does NOT match the corresponding
                // byte from the thumbprint in the BookStoreSTS.com certificate, return false
                if (issuerThumbprint[i] != certThumbprint[i])
                    return false;
            }

            // If we get through all the above checks, return true (ClaimSet was issued by HomeRealmSTS.com)
            return true;
        }
        #endregion
    }
}

