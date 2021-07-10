//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.ExtendedProtection
{
    class Program
    {
        static void Main(string[] args)
        {
            string passCode = "My Personal passcode";

            // Create a client to talk to the service
            GetKeyClient client = new GetKeyClient();
            
            // Call the service 
            // The service call will use the extended protection feature
            //  at the wire level if configured for this by setting the 
            //  extended protection policy setting in the WCF and IIS to 
            //  either 'When Supported' or 'Always'
            Console.WriteLine(String.Format("Your key for '{0}' passcode is '{1}'", 
                passCode, client.GetKeyFromPasscode("My Secret Key")));
            Console.ReadLine();
        }
    }
}
