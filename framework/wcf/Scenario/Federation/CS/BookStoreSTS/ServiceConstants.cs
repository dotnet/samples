//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.Federation
{
    class ServiceConstants
    {
        // Issuer name placed into issued tokens
        internal const string SecurityTokenServiceName = "BookStore STS";

        // Statics for location of certs
        internal static readonly StoreName CertStoreName = StoreName.TrustedPeople;
        internal static readonly StoreLocation CertStoreLocation = StoreLocation.LocalMachine;

        // Statics initialized from app.config
        internal static string CertDistinguishedName;
        internal static string TargetDistinguishedName;
        internal static string IssuerDistinguishedName;
        internal static string BookDB;

        #region Helper functions to load app settings from config
        /// <summary>
        /// Helper function to load Application Settings from config
        /// </summary>
        public static void LoadAppSettings()
        {
            BookDB = ConfigurationManager.AppSettings["bookDB"];
            CheckIfLoaded(BookDB);
            BookDB = String.Format("{0}\\{1}", System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, BookDB);

            CertDistinguishedName = ConfigurationManager.AppSettings["certDistinguishedName"];
            CheckIfLoaded(CertDistinguishedName);

            TargetDistinguishedName = ConfigurationManager.AppSettings["targetDistinguishedName"];
            CheckIfLoaded(TargetDistinguishedName);

            IssuerDistinguishedName = ConfigurationManager.AppSettings["issuerDistinguishedName"];
            CheckIfLoaded(IssuerDistinguishedName);
        }

        /// <summary>
        /// Helper function to check if a required Application Setting has been specified in config.
        /// Throw if some Application Setting has not been specified.
        /// </summary>
        private static void CheckIfLoaded(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                throw new ConfigurationErrorsException("Required Configuration Element(s) missing at BookStoreSTS. Please check the STS configuration file.");
            }
        }

        #endregion

        private ServiceConstants() { }
    }
}

