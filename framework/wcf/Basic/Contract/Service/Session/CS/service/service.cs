
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Session
{
    // Define a service contract which requires a session.
    // ICalculatorSession allows one to perform multiple operations on a running result
    // One can retrieve the current result by calling Result()
    // One can begin calculating a new result by calling Clear()
    [ServiceContract(Namespace="http://Microsoft.Samples.Session", SessionMode=SessionMode.Required)]
    public interface ICalculatorSession
    {
        [OperationContract(IsOneWay=true)]
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

    // Service class which implements the service contract.
    // Use an InstanceContextMode of PrivateSession to store the result
    // An instance of the service will be bound to each session
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
    public class CalculatorService : ICalculatorSession
    {
        double result = 0.0D;

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
