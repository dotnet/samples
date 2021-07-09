//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Diagnostics;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract]
    interface IDoWork
    {
        [OperationContract]
        void DoWork();
    }

    class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press <ENTER> to start the client.");
            Console.ReadLine();

            CallWorkService();
            CallObjectPooledWorkService();

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }

        static void CallWorkService()
        {           
            ChannelFactory<IDoWork> channelFactory = new ChannelFactory<IDoWork>("WorkService");

            IDoWork channel = channelFactory.CreateChannel();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ColorConsole.WriteLine(ConsoleColor.Yellow, "Calling WorkService:");

            // Call the service method for 5 times
            for (int i = 1; i <= 5; i++)
            {
                channel.DoWork();
                ColorConsole.WriteLine(ConsoleColor.Yellow, "{0} - DoWork() Done", i);
            }

            stopwatch.Stop();
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Calling WorkService took: " + stopwatch.ElapsedMilliseconds.ToString() + " ms.");

            ((IClientChannel)channel).Close();
        }

        static void CallObjectPooledWorkService()
        {            
            ChannelFactory<IDoWork> channelFactory = new ChannelFactory<IDoWork>("ObjectPooledWorkService");

            IDoWork channel = channelFactory.CreateChannel();

            ColorConsole.WriteLine(ConsoleColor.Blue, "Calling ObjectPooledWorkService:");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Call the service method for 5 times
            for (int i = 1; i <= 5; i++)
            {
                channel.DoWork();
                ColorConsole.WriteLine(ConsoleColor.Blue, "{0} - DoWork() Done", i);
            }

            stopwatch.Stop();
            ColorConsole.WriteLine(ConsoleColor.Blue, "Calling ObjectPooledWorkService took: " + stopwatch.ElapsedMilliseconds.ToString() + " ms.");

            ((IClientChannel)channel).Close();
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
