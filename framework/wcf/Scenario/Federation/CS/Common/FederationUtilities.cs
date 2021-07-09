//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace Microsoft.Samples.Federation
{
	public sealed class FederationUtilities
	{
        /// <summary>
        /// Utility method to get a certificate from a given store
        /// </summary>
        /// <param name="storeName">Name of certificate store (e.g. My, TrustedPeople)</param>
        /// <param name="storeLocation">Location of certificate store (e.g. LocalMachine, CurrentUser)</param>
        /// <param name="subjectDistinguishedName">The Subject Distinguished Name of the certificate</param>
        /// <returns>The specified X509 certificate</returns>
        static X509Certificate2 LookupCertificate(StoreName storeName, StoreLocation storeLocation, string subjectDistinguishedName)
        {
            X509Store store = null;
            try
            {
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName,
                                                                           subjectDistinguishedName, false);
                if (certs.Count != 1)
                {
                    throw new Exception(String.Format("FedUtil: Certificate {0} not found or more than one certificate found", subjectDistinguishedName));
                }
                return (X509Certificate2)certs[0];
            }
            finally
            {
                if (store != null) store.Close();
            }
        }


		#region GetX509TokenFromCert()
        /// <summary>
        /// Utility method to get a X509 Token from a given certificate
        /// </summary>
        /// <param name="storeName">Name of certificate store (e.g. My, TrustedPeople)</param>
        /// <param name="storeLocation">Location of certificate store (e.g. LocalMachine, CurrentUser)</param>
        /// <param name="subjectDistinguishedName">The Subject Distinguished Name of the certificate</param>
        /// <returns>The corresponding X509 Token</returns>
        public static X509SecurityToken GetX509TokenFromCert(StoreName storeName,
                                                             StoreLocation storeLocation,
                                                             string subjectDistinguishedName)
        {
            X509Certificate2 certificate = LookupCertificate(storeName, storeLocation, subjectDistinguishedName);
            X509SecurityToken t = new X509SecurityToken(certificate);
            return t;
        }
		#endregion

        #region GetCertificateThumbprint
        /// <summary>
        /// Utility method to get an X509 Certificate thumbprint
        /// </summary>
        /// <param name="storeName">Name of certificate store (e.g. My, TrustedPeople)</param>
        /// <param name="storeLocation">Location of certificate store (e.g. LocalMachine, CurrentUser)</param>
        /// <param name="subjectDistinguishedName">The Subject Distinguished Name of the certificate</param>
        /// <returns>The corresponding X509 Certificate thumbprint</returns>
        public static byte[] GetCertificateThumbprint(StoreName storeName, StoreLocation storeLocation, string subjectDistinguishedName)
        {
            X509Certificate2 certificate = LookupCertificate(storeName, storeLocation, subjectDistinguishedName);
            return certificate.GetCertHash();
        }

        #endregion

        private FederationUtilities() { }
	}
}

