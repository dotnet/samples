
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace Microsoft.Samples.UserName
{


    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.UserName")]
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

    public class CalculatorService : ICalculator
    {

        public string GetCallerIdentity()
        {
            // By default the username client credential is mapped to a Windows identity
            // The Windows identity of the caller can be accessed on the ServiceSecurityContext.WindowsIdentity
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

