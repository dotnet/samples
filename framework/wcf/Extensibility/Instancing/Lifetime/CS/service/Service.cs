//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract(SessionMode=SessionMode.Required)]
    public interface IEchoService
    {
        [OperationContract]
        string Echo(string value);
    }

    [CustomLeaseTime(Timeout = 20000)]
    public class EchoService : IEchoService
    {
        public EchoService()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Service instance created.");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        
        public string Echo(string value)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Echo method invoked with :" + value);
            Console.ForegroundColor = ConsoleColor.Gray;

            return value;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PrintLegend();

            ServiceHost host = new ServiceHost(typeof(EchoService));
            host.Open();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Echo service started.");
            Console.ForegroundColor = ConsoleColor.Gray;

	    Console.WriteLine("Press <ENTER> to complete test.");
            Console.ReadLine();
            host.Close();
        }

        static void PrintLegend()
        {
            Console.WriteLine("=================================");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Green: Messages from the service host.");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Blue: Messages from the service instance.");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Red: Messages from custom lease behavior.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("=================================");
            Console.WriteLine("");
        }
    }
}
