//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Messaging;
using System.ServiceModel;

namespace Microsoft.Samples.MSMQMessageCorrelation
{
    public class Program
    {
        // Host the service within this EXE console application.
        public static void Main()
        {
            // Get MSMQ queue name from app settings in configuration
            string queueName = ConfigurationManager.AppSettings["orderQueueName"];

            // Create the transacted MSMQ queue if necessary.
            if (!MessageQueue.Exists(queueName))
                MessageQueue.Create(queueName, true);


            // Create a ServiceHost for the OrderProcessorService type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(OrderProcessorService)))
            {
                //Open the ServiceHost to create listeners and start listening for messages.
                
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.ReadLine();
            }
        }
    }
}


