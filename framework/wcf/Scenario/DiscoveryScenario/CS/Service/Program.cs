//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Discovery
{
    class CalculatorServiceHost
    {
        public static void Main()
        {
            Console.WriteLine(" **** ICalculatorService service ****");
            Uri baseAddress = new Uri("http://localhost:8000/" + Guid.NewGuid().ToString());

            ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService), baseAddress);

            try
            {
                // Discovery is being added through config
                serviceHost.Open();

                Console.WriteLine("Calculator Service started at {0}", baseAddress);
                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to terminate the service.");
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }

            if (serviceHost.State != CommunicationState.Closed)
            {
                Console.WriteLine("Aborting service...");
                serviceHost.Abort();
            }
        }
    }
}
