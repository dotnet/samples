//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.Tools.FindPrivateKey
{
    class FindPrivateKey
    {
        static void PrintHelp()
        {
            Console.WriteLine("FindPrivateKey helps user to find the location of the Private Key file of a X.509 Certificate.");
            Console.WriteLine("Usage: FindPrivateKey <storeName> <storeLocation> [{ {-n <subjectName>} | {-t <thumbprint>} } [-f | -d | -a]]");
            Console.WriteLine("       <subjectName> subject name of the certificate");
            Console.WriteLine("       <thumbprint>  thumbprint of the certificate (use certmgr.exe to get it)");
            Console.WriteLine("       -f            output file name only");
            Console.WriteLine("       -d            output directory only");
            Console.WriteLine("       -a            output absolute file name");
            Console.WriteLine("e.g. FindPrivateKey My CurrentUser -n \"CN=John Doe\"");
            Console.WriteLine("e.g. FindPrivateKey My LocalMachine -t \"03 33 98 63 d0 47 e7 48 71 33 62 64 76 5c 4c 9d 42 1d 6b 52\" -c");
        }

		static void Main(string[] args)
		{            
            if (args.Length < 2 || args.Length == 3 || args.Length > 5 
                || (args.Length > 2 && args[2] != "-n" && args[2] != "-t")
                || (args.Length == 5 && args[4] != "-f" && args[4] != "-d" && args[4] != "-a"))
            {
                PrintHelp();
                return;
            }

            try
            {
                StoreName storeName = (StoreName)Enum.Parse(typeof(StoreName), args[0], true);
                StoreLocation storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), args[1], true);

                X509Certificate2 cert;
                if (args.Length > 2)
                {
                    // insert a comma followed by a space for store.Certificates.Find(findType, key, false) 
                    // to successful find the certificate
                    string key = args[3];
                    string[] keys = key.Split(',');
                    key = string.Empty;
                    for (int i = 0; i < keys.Length; i++)
                    {
                        key += keys[i];
                        if ( i != keys.Length -1 )
                         key += ", ";
                    }
                    if (args[2] == "-n")
                        cert = LoadCertificate(storeName, storeLocation, key, X509FindType.FindBySubjectDistinguishedName);
                    else
                        cert = LoadCertificate(storeName, storeLocation, key, X509FindType.FindByThumbprint);
                }
                else
                {
                    cert = SelectCertificate(storeName, storeLocation);
                    if (cert == null)
                        return;
                }

                string privateKeyFile = GetKeyFileName(cert);
                string privateKeyDirectory = GetKeyFileDirectory(privateKeyFile);

                if (args.Length == 5)
                {
                    if (args[4] == "-f")
                        Console.WriteLine(privateKeyFile);
                    else if (args[4] == "-d")
                        Console.WriteLine(privateKeyDirectory);
                    else
                        Console.WriteLine("{0}\\{1}", privateKeyDirectory, privateKeyFile);
                }
                else
                {
                    Console.WriteLine("Private key directory:");
                    Console.WriteLine(privateKeyDirectory);
                    Console.WriteLine("Private key file name:");
                    Console.WriteLine(privateKeyFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FindPrivateKey failed for the following reason:");
                Console.WriteLine(ex.Message);
                Console.WriteLine("\nUse /? option for help");
            }
        }

        static X509Certificate2 SelectCertificate(StoreName storeName, StoreLocation storeLocation)
        {
            X509Certificate2 result;

            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Collection matches;
		        matches = X509Certificate2UI.SelectFromCollection(store.Certificates, "Select certificate", "Select the certificate to find the location of the associated private key file:", X509SelectionFlag.SingleSelection);
                if (matches.Count != 1)
                    result = null;
                else
                    result = matches[0];
            }
            finally
            {
                store.Close();
            }

            return result;
        }

        static X509Certificate2 LoadCertificate(StoreName storeName, StoreLocation storeLocation, string key, X509FindType findType)
        {
            X509Certificate2 result;

            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Collection matches;
                matches = store.Certificates.Find(findType, key, false);
                if (matches.Count > 1)
                    throw new InvalidOperationException(String.Format("More than one certificate with key '{0}' found in the store.", key));
                if (matches.Count == 0)
                    throw new InvalidOperationException(String.Format("No certificates with key '{0}' found in the store.", key));
                result = matches[0];
            }
            finally
            {
                store.Close();
            }

            return result;
        }

		static string GetKeyFileName(X509Certificate2 cert)
		{
			IntPtr            hProvider     = IntPtr.Zero; // CSP handle
			bool              freeProvider  = false;       // Do we need to free the CSP ?
			uint               acquireFlags  = 0;
			int			 	  _keyNumber = 0;
			string			  keyFileName = null;
			byte[]			  keyFileBytes = null;

			//
			// Determine whether there is private key information available for this certificate in the key store
			//
			if ( CryptAcquireCertificatePrivateKey(cert.Handle,
				acquireFlags,
				IntPtr.Zero,
				ref hProvider,
				ref _keyNumber,
				ref freeProvider) )
			{
				IntPtr pBytes  = IntPtr.Zero; // Native Memory for the CRYPT_KEY_PROV_INFO structure
				int    cbBytes = 0;           // Native Memory size

				try
				{
					if ( CryptGetProvParam(hProvider, CryptGetProvParamType.PP_UNIQUE_CONTAINER, IntPtr.Zero, ref cbBytes, 0) )
					{
						pBytes = Marshal.AllocHGlobal(cbBytes);

						if ( CryptGetProvParam(hProvider, CryptGetProvParamType.PP_UNIQUE_CONTAINER, pBytes, ref cbBytes, 0) )
						{
							keyFileBytes = new byte[cbBytes];

							Marshal.Copy(pBytes,keyFileBytes,0,cbBytes);

							// Copy everything except tailing null byte
							keyFileName = System.Text.Encoding.ASCII.GetString(keyFileBytes, 0, keyFileBytes.Length-1);
						}
					}		
				}
				finally
				{
					if ( freeProvider )
						CryptReleaseContext(hProvider,0);

					//
					// Free our native memory
					//
					if ( pBytes != IntPtr.Zero )
						Marshal.FreeHGlobal(pBytes);

				}
			}

            if (keyFileName == null)
                throw new InvalidOperationException("Unable to obtain private key file name");

            return keyFileName;
		}

        static string GetKeyFileDirectory(string keyFileName)
        {
            // Look up All User profile from environment variable
            string allUserProfile = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // set up searching directory
            string machineKeyDir = allUserProfile + "\\Microsoft\\Crypto\\RSA\\MachineKeys";

            // Search the key file
            string[] fs = System.IO.Directory.GetFiles(machineKeyDir, keyFileName);

            // If found
            if (fs.Length > 0)
                return machineKeyDir;

            // Next try current user profile
            string currentUserProfile = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Search all subdirectories.
            string userKeyDir = currentUserProfile + "\\Microsoft\\Crypto\\RSA\\";

            fs = System.IO.Directory.GetDirectories(userKeyDir);
            if (fs.Length > 0)
            {
                // for each sub directory
                foreach (string keyDir in fs)
                {
                    fs = System.IO.Directory.GetFiles(keyDir, keyFileName);
                    if (fs.Length == 0)
                        continue;
                    else
                        // found
                        return keyDir;
                }
            }

            throw new InvalidOperationException("Unable to locate private key file directory");
        }

        [DllImport("crypt32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal extern static bool CryptAcquireCertificatePrivateKey(IntPtr pCert, uint dwFlags, IntPtr pvReserved, ref IntPtr phCryptProv, ref int pdwKeySpec, ref bool pfCallerFreeProv);

        [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal extern static bool CryptGetProvParam(IntPtr hCryptProv, CryptGetProvParamType dwParam, IntPtr pvData, ref int pcbData, uint dwFlags);

        [DllImport("advapi32", SetLastError = true)]
        internal extern static bool CryptReleaseContext(IntPtr hProv, uint dwFlags);

    }

    enum CryptGetProvParamType
    {
        PP_ENUMALGS = 1,
        PP_ENUMCONTAINERS = 2,
        PP_IMPTYPE = 3,
        PP_NAME = 4,
        PP_VERSION = 5,
        PP_CONTAINER = 6,
        PP_CHANGE_PASSWORD = 7,
        PP_KEYSET_SEC_DESCR = 8,       // get/set security descriptor of keyset
        PP_CERTCHAIN = 9,      // for retrieving certificates from tokens
        PP_KEY_TYPE_SUBTYPE = 10,
        PP_PROVTYPE = 16,
        PP_KEYSTORAGE = 17,
        PP_APPLI_CERT = 18,
        PP_SYM_KEYSIZE = 19,
        PP_SESSION_KEYSIZE = 20,
        PP_UI_PROMPT = 21,
        PP_ENUMALGS_EX = 22,
        PP_ENUMMANDROOTS = 25,
        PP_ENUMELECTROOTS = 26,
        PP_KEYSET_TYPE = 27,
        PP_ADMIN_PIN = 31,
        PP_KEYEXCHANGE_PIN = 32,
        PP_SIGNATURE_PIN = 33,
        PP_SIG_KEYSIZE_INC = 34,
        PP_KEYX_KEYSIZE_INC = 35,
        PP_UNIQUE_CONTAINER = 36,
        PP_SGC_INFO = 37,
        PP_USE_HARDWARE_RNG = 38,
        PP_KEYSPEC = 39,
        PP_ENUMEX_SIGNING_PROT = 40,
        PP_CRYPT_COUNT_KEY_USE = 41,
    }
}
