
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples")]
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
    public class CalculatorService : ICalculator
    {
        public double Add(double n1, double n2)
        {
            return n1 + n2;
        }

        public double Subtract(double n1, double n2)
        {
            return n1 - n2;
        }

        public double Multiply(double n1, double n2)
        {
            return n1 * n2;
        }

        public double Divide(double n1, double n2)
        {
            return n1 / n2;
        }

        // Host the service within this EXE console application.
        public static void Main()
        {
            Uri baseAddress = new Uri("http://localhost:8000/servicemodelsamples/service");

            // Create a ServiceHost for the CalculatorService type and provide the base address.
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService), baseAddress))
            {
                // Create a custom binding containing two binding elements
                ReliableSessionBindingElement reliableSession = new ReliableSessionBindingElement();
                reliableSession.Ordered = true;

                HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
                httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
                httpTransport.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

                CustomBinding binding = new CustomBinding(reliableSession, httpTransport);

                // Add an endpoint using that binding
                serviceHost.AddServiceEndpoint(typeof(ICalculator), binding, "");
              
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

	
  

