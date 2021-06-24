//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Messaging;
using System.ServiceModel;

namespace Microsoft.Samples.MSMQVolatileSample
{
    // Define a service contract. 
    [ServiceContract(Namespace = "http://Microsoft.Samples.MSMQVolatileSample")]
    public interface IStockTicker
    {
        [OperationContract(IsOneWay = true)]
        void StockTick(string symbol, float price);
    }

    // Service class which implements the service contract.
    // Added code to write output to the console window
    public class StockTickerService : IStockTicker
    {
        public void StockTick(string symbol, float price)
        {
            Console.WriteLine("Stock Tick {0}:{1} ", symbol, price);
        }

        // Host the service within this EXE console application.
        public static void Main()
        {
            // Get MSMQ queue name from app settings in configuration
            string queueName = ConfigurationManager.AppSettings["queueName"];

            // Create the transacted MSMQ queue if necessary.
            if (!MessageQueue.Exists(queueName))
                MessageQueue.Create(queueName);

            // Create a ServiceHost for the StockTickerService type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(StockTickerService)))
            {
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

    }

}


