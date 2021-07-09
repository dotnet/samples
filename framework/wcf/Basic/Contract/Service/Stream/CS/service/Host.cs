
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Configuration;

 namespace Microsoft.Samples.Stream
{
    class Host
    {
        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the StreamingService type
            using (ServiceHost serviceHost = new ServiceHost(typeof(StreamingService)))
            {
                // Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The streaming service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
