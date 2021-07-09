
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.TransportWithMessageCredentialSecurity
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.TransportWithMessageCredentialSecurity")]
    public interface ICalculator
    {
        [OperationContract]
        string GetCallerIdentity();
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
    // Added code to access identity of the caller
    public class CalculatorService : ICalculator
    {

        public string GetCallerIdentity()
        {
            // use ServiceSecurityContext.WindowsIdentity to get the name of the caller
            return ServiceSecurityContext.Current.WindowsIdentity.Name;
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

