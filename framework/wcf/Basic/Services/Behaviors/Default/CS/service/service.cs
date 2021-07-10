
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Behaviors
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.Behaviors", SessionMode = SessionMode.Required)]
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

    [ServiceBehavior(
        AutomaticSessionShutdown=true,
        ConcurrencyMode=ConcurrencyMode.Single,
        InstanceContextMode=InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults=false,
        UseSynchronizationContext=true,
        ValidateMustUnderstand=true)]
    public class CalculatorService : ICalculator
    {
        [OperationBehavior(
            TransactionAutoComplete=true,
            TransactionScopeRequired=false,
            Impersonation=ImpersonationOption.NotAllowed)]
        public double Add(double n1, double n2)
        {
            System.Threading.Thread.Sleep(1600);
            return n1 + n2;
        }

        [OperationBehavior(
            TransactionAutoComplete = true,
            TransactionScopeRequired = false,
            Impersonation = ImpersonationOption.NotAllowed)]
        public double Subtract(double n1, double n2)
        {
            System.Threading.Thread.Sleep(800);
            return n1 - n2;
        }

        [OperationBehavior(
            TransactionAutoComplete = true,
            TransactionScopeRequired = false,
            Impersonation = ImpersonationOption.NotAllowed)]
        public double Multiply(double n1, double n2)
        {
            System.Threading.Thread.Sleep(400);
            return n1 * n2;
        }

        [OperationBehavior(
            TransactionAutoComplete = true,
            TransactionScopeRequired = false,
            Impersonation = ImpersonationOption.NotAllowed)]
        public double Divide(double n1, double n2)
        {
            System.Threading.Thread.Sleep(100);
            return n1 / n2;
        }
    }

}
