
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;

namespace Microsoft.Samples.PrincipalPermissionAuthorization
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.PrincipalPermissionAuthorization")]
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
    // Added PrincipalPermission attributes to authorize administrators to access each operation
    public class CalculatorService : ICalculator
    {
        [PrincipalPermission(SecurityAction.Demand, Role = "Builtin\\Administrators")]
        public double Add(double n1, double n2)
        {
            double result = n1 + n2;
            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Builtin\\Administrators")]
        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;
            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Builtin\\Administrators")]
        public double Multiply(double n1, double n2)
        {
            double result = n1 * n2;
            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Builtin\\Administrators")]
        public double Divide(double n1, double n2)
        {
            double result = n1 / n2;
            return result;
        }

    }

}

