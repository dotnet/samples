
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Description;


namespace Microsoft.Samples.Addressing
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.Addressing")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

    // Service class which implements the service contract.
    // Added code to write output to the console window
    public class CalculatorService : ICalculator
    {
        public double Add(double n1, double n2)
        {
            double result = n1 + n2;
            Console.WriteLine("Received Add({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;
            Console.WriteLine("Received Subtract({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Multiply(double n1, double n2)
        {
            double result = n1 * n2;
            Console.WriteLine("Received Multiply({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Divide(double n1, double n2)
        {
            double result = n1 / n2;
            Console.WriteLine("Received Divide({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }


        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService), new Uri("http://localhost:8000/ServiceModelSamples/service"), new Uri("net.tcp://localhost:9000/servicemodelsamples/service")))
            {
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();

                // Enumerate endpoints
                Console.WriteLine("Service endpoints:");
                ServiceDescription desc = serviceHost.Description;
                foreach (ServiceEndpoint endpoint in desc.Endpoints)
                {
                    Console.WriteLine("Endpoint - address:  {0}", endpoint.Address);
                    Console.WriteLine("           binding:  {0}", endpoint.Binding.Name);
                    Console.WriteLine("           contract: {0}", endpoint.Contract.Name);
                }

                Console.WriteLine();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

    }

}
