
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.ClientValidation
{
    [ServiceContract(Namespace = "http://Microsoft.Samples.ClientValidation")]
    public interface ICalculator
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        double Add(double n1, double n2);
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        double Subtract(double n1, double n2);
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        double Multiply(double n1, double n2);
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        double Divide(double n1, double n2);
    }

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

        public static void Main()
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService)))
            {
                serviceHost.Open();

                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

    }

}

