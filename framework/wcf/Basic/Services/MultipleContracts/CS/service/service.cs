
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

    // Define a second service contract.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples", SessionMode=SessionMode.Required)]
    public interface ICalculatorSession
    {
        [OperationContract(IsOneWay = true)]
        void Clear();
        [OperationContract(IsOneWay = true)]
        void AddTo(double n);
        [OperationContract(IsOneWay = true)]
        void SubtractFrom(double n);
        [OperationContract(IsOneWay = true)]
        void MultiplyBy(double n);
        [OperationContract(IsOneWay = true)]
        void DivideBy(double n);
        [OperationContract]
        double Result();
    }

    // Service class which implements the two contracts.
    // Use an InstanceContextMode of PerSession to maintain the result
    // An instance of the service will be bound to each session
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class CalculatorService : ICalculator, ICalculatorSession
    {
        // Implementation of ICalculator
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


        double result;
        // Implementation of ICalculatorSession
        public void Clear()
        {
            result = 0.0D;
        }

        public void AddTo(double n)
        {
            result += n;
        }

        public void SubtractFrom(double n)
        {
            result -= n;
        }

        public void MultiplyBy(double n)
        {
            result *= n;
        }

        public void DivideBy(double n)
        {
            result /= n;
        }

        public double Result()
        {
            return result;
        }

    }

}

