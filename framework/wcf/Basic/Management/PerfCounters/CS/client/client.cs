
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Threading;
using System.Diagnostics;
namespace Microsoft.Samples.ServiceModel
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        //the service exits when the flag becomes true
        static bool flag;

        static void Main()
        {
            CreateCounter();
            Thread workerThread = new Thread(new ThreadStart(DoWork));

            workerThread.Start();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client once the output starts displaying.");
            Console.ReadLine();
            flag = true;

            workerThread.Join();
        }
        public static void CreateCounter()
        {
            CounterCreationDataCollection col = new CounterCreationDataCollection();

            // Create two custom counter objects.
            CounterCreationData addCounter = new CounterCreationData();
            addCounter.CounterName = "AddCounter";
            addCounter.CounterHelp = "Custom Add counter ";
            addCounter.CounterType = PerformanceCounterType.NumberOfItemsHEX32;

            // Add custom counter objects to CounterCreationDataCollection.
            col.Add(addCounter);

            // Bind the counters to a PerformanceCounterCategory
            // Check if the category already exists or not.
            if (!PerformanceCounterCategory.Exists("MyCategory"))
            {
                PerformanceCounterCategory category =
                PerformanceCounterCategory.Create("MyCategory", "My Perf Category Description ", PerformanceCounterCategoryType.Unknown, col);
            }
            else
            {
                Console.WriteLine("Counter already exists");
            }
        }
        static void IncrementCounter()
        {
            // get an instance of our perf counter
            PerformanceCounter counter = new PerformanceCounter();
            counter.CategoryName = "MyCategory";
            counter.CounterName = "AddCounter";
            counter.ReadOnly = false;

            // increment and close the perf counter
            counter.Increment();

            counter.Close();
        }

        static void DoWork()
        {
            // Create a client with given client endpoint configuration
            CalculatorClient client = new CalculatorClient();

            Random rand = new Random();

            while (!flag)
            {
                // Call the Add service operation.
                double value1 = (double)rand.Next(0, 5);
                double value2 = (double)rand.Next(0, 5);
                double result;

                switch (rand.Next(0, 4))
                {
                    case (0):
                        IncrementCounter();
                        result = client.Add(value1, value2);
                        Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);
                        break;
                    case (1):
                        result = client.Subtract(value1, value2);
                        Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);
                        break;
                    case (2):
                        result = client.Multiply(value1, value2);
                        Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);
                        break;
                    case (3):
                        result = client.Divide(value1, value2);
                        Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
                        break;
                }

                Thread.Sleep(500);
            }

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }
    }
}
