
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
	// Define a service contract.
	[ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
	public interface ICalculator
	{
		[OperationContract(IsOneWay=false)]
		double Add(double n1, double n2);
        [OperationContract(IsOneWay = false)]
		double Subtract(double n1, double n2);
        [OperationContract(IsOneWay = false)]
		double Multiply(double n1, double n2);
        [OperationContract(IsOneWay = false)]
		double Divide(double n1, double n2);
	}

	// Service class which implements the service contract.
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
            WSHttpBinding binding = new WSHttpBinding();
            binding.Name = "binding1";
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.Security.Mode = SecurityMode.Message;
            binding.ReliableSession.Enabled = false;
            binding.TransactionFlow = false;
            
            Uri baseAddress = new Uri("http://localhost:8000/servicemodelsamples/service");

            // Create a ServiceHost for the CalculatorService type and provide the base address.
            using(ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService), baseAddress))
            {
                serviceHost.AddServiceEndpoint(typeof(ICalculator), binding, baseAddress);
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

