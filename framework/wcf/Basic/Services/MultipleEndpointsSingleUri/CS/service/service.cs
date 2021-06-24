
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
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

    // Define a second contract
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface IEcho
    {
        [OperationContract]
        string Echo(string s);
    }

    // Service class which implements the service contracts.
    public class CalculatorService : ICalculator, IEcho
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

        public string Echo(string s)
        {
            string addressIncomingMessageWasSentTo = OperationContext.Current.IncomingMessageHeaders.To.ToString();
            return s + "\n(Message was sent To " + addressIncomingMessageWasSentTo + ")";
        }
    }

}

