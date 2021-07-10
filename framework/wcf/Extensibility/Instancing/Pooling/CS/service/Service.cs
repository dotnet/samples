//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Threading;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract]
    interface IDoWork
    {
        [OperationContract]
        void DoWork();
    }

    // This class takes a lot of time for the initialization, but it has 
    // short method calls.
    public class WorkService : IDoWork
    {
        public WorkService()
        {
            Thread.Sleep(5000);

            ColorConsole.WriteLine(ConsoleColor.Yellow, "WorkService instance created.");
        }

        public void DoWork()
        {
            ColorConsole.WriteLine(ConsoleColor.Yellow, "WorkService.GetData() completed.");
        }        
    }
     
    // This class takes a lot of time for the initialization, but it has 
    // short method calls.  It uses object pooling.
    [ObjectPooling(MinPoolSize = 0, MaxPoolSize = 5)]
    public class ObjectPooledWorkService : IDoWork
    {
        public ObjectPooledWorkService()
        {
            Thread.Sleep(5000);

            ColorConsole.WriteLine(ConsoleColor.Blue, "ObjectPooledWorkService instance created.");
        }

        public void DoWork()
        {
            ColorConsole.WriteLine(ConsoleColor.Blue, "ObjectPooledWorkService.GetData() completed.");
        }        
    }

    class Service
    {
        static void Main(string[] args)
        {
            PrintLegend();

            // Configure and open the host gateway
            ServiceHost host1 = new ServiceHost(typeof(WorkService));
            host1.Open();

            ColorConsole.WriteLine(ConsoleColor.Green, "WorkService is running.");

            // Configure and open the local gateway
            ServiceHost host2 = new ServiceHost(typeof(ObjectPooledWorkService));
            host2.Open();

            ColorConsole.WriteLine(ConsoleColor.Green, "ObjectPooledWorkService is running.");

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
            host1.Close();
            host2.Close();
        }

        static void PrintLegend()
        {
            Console.WriteLine("=================================");

            ColorConsole.WriteLine(ConsoleColor.Green, "Green: Messages from the ServiceHost.");

            ColorConsole.WriteLine(ConsoleColor.Yellow, "Yellow: Messages from WorkService instance.");

            ColorConsole.WriteLine(ConsoleColor.Blue, "Blue: Messages from ObjectPooledWorkService instance.");

            ColorConsole.WriteLine(ConsoleColor.Red, "Red: Messages from the object pool.");

            Console.WriteLine("=================================");
            Console.WriteLine();
        }
    }

    internal static class ColorConsole
    {
        public static void WriteLine(ConsoleColor color, string text, params object[] args)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, args);
            Console.ForegroundColor = currentColor;
        }
    }
}
