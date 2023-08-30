//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.PortSharing
{
    // Define a service contract
    [ServiceContract(Namespace = "http://Microsoft.Samples.PortSharing")]
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

    // Service class that implements the service contract
    public class CalculatorService : ICalculator
    {
        public double Add(double n1, double n2) { return n1 + n2; }
        public double Subtract(double n1, double n2) { return n1 - n2; }
        public double Multiply(double n1, double n2) { return n1 * n2; }
        public double Divide(double n1, double n2) { return n1 / n2; }
    }

    class service
    {
        static void Main(string[] args)
        {
            // Configure a binding with TCP port sharing enabled
            NetTcpBinding binding = new NetTcpBinding();
            binding.PortSharingEnabled = true;

            // Start a service on a fixed TCP port
            ServiceHost host = new ServiceHost(typeof(CalculatorService));
            ushort salt = (ushort)new Random().Next();
            string address = string.Format("net.tcp://localhost:9000/calculator/{0}", salt);
            host.AddServiceEndpoint(typeof(ICalculator), binding, address);
            host.Open();
            Console.WriteLine("Service #{0} listening on {1}.", salt, address);
            Console.WriteLine(string.Format("Client should use service number {0} to communicate with the service.", salt));
            Console.WriteLine("Press <ENTER> to terminate service.");
            Console.ReadLine();
            host.Close();
        }
    }
}
