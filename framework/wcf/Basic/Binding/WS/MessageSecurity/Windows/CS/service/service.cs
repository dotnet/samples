
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;

namespace Microsoft.Samples.Windows
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.Windows")]
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
    // Added code to access Windows identity using the Thread.CurrentPrinciple property.
    public class CalculatorService : ICalculator
    {
        public string GetCallerIdentity()
        {
            // The Windows identity of the caller can be accessed on the ServiceSecurityContext.WindowsIdentity
            return OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
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

