//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.Discovery
{

    static class CertificateHelper
    {
        static ClientCertificateCache certificatesCache = new ClientCertificateCache();

        public static string GetKeyId(X509Certificate2 certificate)
        {
            return certificate.Thumbprint;
        }

        public static X509Certificate2 GetCertificateByThumbprint(
            StoreName storeName, 
            StoreLocation storeLocation, 
            string thumbPrint)
        {
            return CertificateHelper.GetCertificate(storeName, storeLocation, X509FindType.FindByThumbprint, thumbPrint);
        }

        public static X509Certificate2 GetCertificate(
            StoreName storeName,
            StoreLocation storeLocation,
            X509FindType findType, 
            string findValue)
        {
            X509Certificate2 certificate = null;

            if (certificatesCache.TryGet(storeName, storeLocation, findType, findValue, out certificate))
            {
                return certificate;
            }

            X509Store store = null; 
            X509Certificate2Collection certificates = null;
            try
            {
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly);

                certificates = store.Certificates;
                if (findType == X509FindType.FindBySubjectName)
                {
                    certificate = CertificateHelper.FindBySubjectName(certificates, findValue);
                }
                else if (findType == X509FindType.FindByThumbprint)
                {
                    certificate = CertificateHelper.FindByThumbprint(certificates, findValue);
                }
                else if (findType == X509FindType.FindBySubjectDistinguishedName)
                {
                    certificate = CertificateHelper.FindBySubjectDistinguishedName(certificates, findValue);
                }
                else
                {
                    throw new CompactSignatureSecurityException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "The value {0} of X509FindType is not supported. Please use FindBySubjectName or FindByThumbprint instead.",
                            findType));
                }
            }
            finally
            {
                if (certificates != null)
                {
                    for(int i = 0; i < certificates.Count; ++i)
                    {
                        certificates[i].Reset();
                    }
                }
                if (store != null)
                {
                    store.Close();
                }
            }

            if (certificate != null)
            {
                certificatesCache.Add(storeName, storeLocation, findType, findValue, certificate);
                return certificate;
            }
            
            throw new CompactSignatureSecurityException(
                string.Format(
                    CultureInfo.CurrentCulture, 
                    "No matching certificates were found for KeyId={0} and X509FindType={1}", 
                    findValue, 
                    findType));
         } 

         static X509Certificate2 FindBySubjectName(X509Certificate2Collection certificates, string findValue)
         {
            foreach(X509Certificate2 certificate in certificates)
            {
                if (certificate.SubjectName.Name.ToLower().Contains(findValue.ToLower()))
                {
                    return new X509Certificate2(certificate);
                }
            }

            return null;
         }

         static X509Certificate2 FindBySubjectDistinguishedName(X509Certificate2Collection certificates, string findValue)
         {
             foreach (X509Certificate2 certificate in certificates)
             {
                 if (string.Compare(certificate.SubjectName.Name, findValue, StringComparison.OrdinalIgnoreCase) == 0)
                 {
                     return new X509Certificate2(certificate);
                 }
             }

             return null;
         }

         static X509Certificate2 FindByThumbprint(X509Certificate2Collection certificates, string findValue)
         {
            foreach(X509Certificate2 certificate in certificates)
            {
                if (string.Compare(certificate.Thumbprint, findValue, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return new X509Certificate2(certificate);
                }
            }

            return null;
         }


        class ClientCertificateCache
        {
            Dictionary<string, X509Certificate2> cache;
            Object thisLock;

            public ClientCertificateCache()
            {
                this.cache = new Dictionary<string, X509Certificate2>();
                this.thisLock = new Object();
            }

            public void Add(
                StoreName storeName, 
                StoreLocation storeLocation, 
                X509FindType findType, 
                string findValue, 
                X509Certificate2 certificate)
            {
                string key = ClientCertificateCache.GetKey(storeName, storeLocation, findType, findValue);

                lock (this.thisLock)
                {
                    this.cache[key] = certificate;
                }
            }

            public bool TryGet(
                StoreName storeName, 
                StoreLocation storeLocation,
                X509FindType findType, 
                string findValue, 
                out X509Certificate2 certificate)
            {
                string key = ClientCertificateCache.GetKey(storeName, storeLocation, findType, findValue);
                lock (this.thisLock)
                {
                    this.cache.TryGetValue(key, out certificate);    
                }

                return (certificate != null);
            }

            static string GetKey(StoreName storeName, StoreLocation storeLocation, X509FindType findType, string findValue)
            {
                return String.Format("{0}{1}{2}{3}", storeName, storeLocation, findType, findValue.ToLower());
            }
        }
    }
}
