
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.ServiceModel;

namespace Microsoft.Samples.OneWay
{
    // Define a service contract. 
    [ServiceContract(Namespace = "http://Microsoft.Samples.OneWay")]
    public interface IOneWayCalculator
    {
        [OperationContract(IsOneWay = true)]
        void Add(double n1, double n2);
        [OperationContract(IsOneWay = true)]
        void Subtract(double n1, double n2);
        [OperationContract(IsOneWay = true)]
        void Multiply(double n1, double n2);
        [OperationContract(IsOneWay = true)]
        void Divide(double n1, double n2);
    }

    // Service class which implements the service contract.
    // Added code to write output to the console window
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class CalculatorService : IOneWayCalculator
    {
        public void Add(double n1, double n2)
        {
            Console.WriteLine("Received Add({0},{1}) - sleeping", n1, n2);
            System.Threading.Thread.Sleep(1000 * 5);
            double result = n1 + n2;
            Console.WriteLine("Processing Add({0},{1}) - result: {2}", n1, n2, result);
        }

        public void Subtract(double n1, double n2)
        {
            Console.WriteLine("Received Subtract({0},{1}) - sleeping", n1, n2);
            System.Threading.Thread.Sleep(1000 * 5);
            double result = n1 - n2;
            Console.WriteLine("Processing Subtract({0},{1}) - result: {2}", n1, n2, result);
        }

        public void Multiply(double n1, double n2)
        {
            Console.WriteLine("Received Multiply({0},{1}) - sleeping", n1, n2);
            System.Threading.Thread.Sleep(1000 * 5);
            double result = n1 * n2;
            Console.WriteLine("Processing Multiply({0},{1}) - result: {2}", n1, n2, result);
        }

        public void Divide(double n1, double n2)
        {
            Console.WriteLine("Received Divide({0},{1}) - sleeping", n1, n2);
            System.Threading.Thread.Sleep(1000 * 5);
            double result = n1 / n2;
            Console.WriteLine("Processing Divide({0},{1}) - result: {2}", n1, n2, result);
        }


        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type and provide the base address.
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService), new Uri("http://localhost:8000/ServiceModelSamples/Service")))
            {
                // Open the ServiceHostBase to create listeners and start listening for messages.
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
