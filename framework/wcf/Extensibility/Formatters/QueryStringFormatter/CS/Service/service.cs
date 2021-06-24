
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Configuration;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Text;

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
    }

    class Host
    {
        static void Main()
        {
            Host app = new Host();
            app.Run();
        }

        private void Run()
        {
            // Create a ServiceHost for the service type and use base address in the configuration file.
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService)))
            {
                // Apply query string formatter
                foreach (ServiceEndpoint endpoint in serviceHost.Description.Endpoints)
                {
                    foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
                    {
                        EnableHttpGetRequestsBehavior.ReplaceFormatterBehavior(operationDescription, endpoint.Address);
                    }
                }

                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
        }
    }

}
