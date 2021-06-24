
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;


namespace Microsoft.Samples.Anonymous
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.Anonymous")]
    public interface ICalculator
    {
        [OperationContract]
        bool IsCallerAnonymous();
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
    // Added code to return whether the caller is anonymous
    public class CalculatorService : ICalculator
    {
        public bool IsCallerAnonymous()
        {
            // ServiceSecurityContext.IsAnonymous returns true if the caller is not authenticated
            return ServiceSecurityContext.Current.IsAnonymous;
        }

        public double Add(double n1, double n2)
        {
            double result = n1 + n2;
            return result;
        }

        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;
            return result;
        }

        public double Multiply(double n1, double n2)
        {
            double result = n1 * n2;
            return result;
        }

        public double Divide(double n1, double n2)
        {
            double result = n1 / n2;
            return result;
        }
    }

}

