//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;

using System.ServiceModel;

namespace Microsoft.Samples.Federation
{
	public class BuyAuthorizationManager : ServiceAuthorizationManager 
	{
		#region AccessCheck() override
        /// <summary>
        /// Implementation of the framework level access control mechanism via ServiceAuthorizationManager
        /// </summary>
        /// <returns>True on success. False on failure.</returns>
        public override bool CheckAccess(OperationContext operationContext)
		{
            // BrowseBooks is always authorized, so return true
            if (operationContext.IncomingMessageHeaders.Action == ServiceConstants.BrowseBooksAction)
                return true;

            // If the requested operation is NOT BuyBook, return false (Access Denied)
            // The only operation we support apart from BrowseBooks is BuyBook
            if (operationContext.IncomingMessageHeaders.Action != ServiceConstants.BuyBookAction)
                return false;

            // Extract the AuthorizationContext from the ServiceSecurityContext
            AuthorizationContext authContext = operationContext.ServiceSecurityContext.AuthorizationContext;

            // If there are no Claims in the AuthorizationContext, return false (Access Denied)
            // The issued token used to authenticate should contain claims 
            if (authContext.ClaimSets == null) 
                return false;

            // If there is more than one ClaimSet in the AuthorizationContext, return false (Access Denied).
            // The issued token used to authenticate should only contain a single set of claims.
            if (authContext.ClaimSets.Count != 2) 
                return false;

            // Extract the single ClaimSet from the AuthorizationContext
            List<ClaimSet> claimsets = new List<ClaimSet>(authContext.ClaimSets);
            ClaimSet myClaimSet = claimsets.Find((Predicate<ClaimSet>)delegate(ClaimSet target)
            {
                X509CertificateClaimSet certClaimSet = target.Issuer as X509CertificateClaimSet;
                return certClaimSet != null && certClaimSet.X509Certificate.Subject == "CN=BookStoreSTS.com";
            });

            // Is the ClaimSet was NOT issued by the BookStoreSTS then return false (Access Denied)
            // The BookStoreService only accepts requests where the client authenticated using a token
            // issued by the BookStoreSTS.
            if (!IssuedByBookStoreSTS(myClaimSet)) 
                return false;

            // Find all the PurchaseAuthorized claims
            IEnumerable<Claim> purchaseAllowedClaims = myClaimSet.FindClaims(Constants.PurchaseAuthorizedClaim, Rights.PossessProperty);

            // If there are no PurchaseAuthorized claims, return false (Access Denied)
            // The BookStoreSTS issues tokens containing PurchaseAuthorized claims for all authorized requests.
            if (purchaseAllowedClaims == null) 
                return false;

            // Get the book name being purchased...
            string bookName = operationContext.IncomingMessageHeaders.GetHeader<string>(Constants.BookNameHeaderName, Constants.BookNameHeaderNamespace);
            // ..and if it's null or empty, return false (Access Denied)
            if (string.IsNullOrEmpty(bookName))
                return false;

            // Iterate through the PurchaseAllowed claims and verify that the Resource value is 
            // the same as the book name retrieved above
            foreach (Claim claim in purchaseAllowedClaims)
            {
                string authorizedBook = claim.Resource.ToString();
                if (!string.IsNullOrEmpty(authorizedBook) && authorizedBook.Trim().Equals(bookName.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // If no PurchaseAllowed claim had a resource value that matched the 
            // book name, return false (Access Denied)
            return false;
        }

        #endregion

        #region IssuedByBookStoreSTS
        /// <summary>
        /// Helper function to check if claims were issued by BookStoreSTS
        /// </summary>
        /// <returns>True on success. False on failure.</returns>
        private static bool IssuedByBookStoreSTS(ClaimSet myClaimSet)
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
            byte[] thumbprint = (byte[])issuerClaim.Resource;

            // Extract the thumbprint for the BookStoreSTS.com certificate
            byte[] issuerCertThumbprint = FederationUtilities.GetCertificateThumbprint(ServiceConstants.CertStoreName,
                                                                           ServiceConstants.CertStoreLocation,
                                                                           ServiceConstants.IssuerCertDistinguishedName);

            // If the lengths of the two thumbprints are different, return false
            if (thumbprint.Length != issuerCertThumbprint.Length) 
                return false;

            // Check the individual bytes of the two thumbprints for equality...
            for (int i = 0; i < thumbprint.Length; i++)
            {
                //... if any byte in the thumbprint from the claim does NOT match the corresponding
                // byte from the thumbprint in the BookStoreSTS.com certificate, return false
                if (thumbprint[i] != issuerCertThumbprint[i]) 
                    return false;
            }

            // If we get through all the above checks, return true (ClaimSet was issued by BookStoreSTS.com)
            return true;
        }

		#endregion
	}
}
