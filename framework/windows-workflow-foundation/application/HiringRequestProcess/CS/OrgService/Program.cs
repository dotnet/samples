//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.ServiceModel;

namespace Microsoft.Samples.Organization
{
    class Program
    {
        public static void Main()
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(OrgService)))
            {
                // Open the ServiceHost to start listening for messages.
                Console.WriteLine("Opening Org Data Service service host...");
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The Org Data Service is ready.");
                Console.WriteLine(string.Format("Address: {0}", serviceHost.BaseAddresses[0].ToString()));
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.ReadLine();

                // Close the ServiceHost.
                serviceHost.Close();
            }
        }
    }
}
