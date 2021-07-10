
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Concurrency
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.Concurrency")]
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

    // Define a service contract to inspect concurrency state
    [ServiceContract(Namespace = "http://Microsoft.Samples.Concurrency")]
    public interface ICalculatorConcurrency : ICalculator
    {
        [OperationContract]
        string GetConcurrencyMode();
        [OperationContract]
        int GetOperationCount();
    }

    // Concurrency controls whether a service instance can process more than one message at a time.
    // PerCall instancing creates a service instance per message, so concurrency is not relevant.
    // Single instancing create a single service instance for all messages.
    // PerSession instancing creates a service instance per session.
    // Enable one of the following concurrency modes to compare behavior.

    // Single allows a single message to be processed sequentially by each service instance.
    //[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]

    // Multiple allows concurrent processing of multiple messages by a service instance.
    // The service implementation should be thread-safe. This can be used to increase throughput.
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]

    // Uses Thread.Sleep to vary the execution time of each operation
    public class CalculatorService : ICalculatorConcurrency
    {
        int operationCount;

        public double Add(double n1, double n2)
        {
            operationCount++;
            System.Threading.Thread.Sleep(180);
            return n1 + n2;
        }

        public double Subtract(double n1, double n2)
        {
            operationCount++;
            System.Threading.Thread.Sleep(100);
            return n1 - n2;
        }

        public double Multiply(double n1, double n2)
        {
            operationCount++;
            System.Threading.Thread.Sleep(150);
            return n1 * n2;
        }

        public double Divide(double n1, double n2)
        {
            operationCount++;
            System.Threading.Thread.Sleep(120);
            return n1 / n2;
        }

        public string GetConcurrencyMode()
        {   
            // Return the ConcurrencyMode of the service
            ServiceHost host = (ServiceHost)OperationContext.Current.Host;
            ServiceBehaviorAttribute behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            return behavior.ConcurrencyMode.ToString();
        }

        public int GetOperationCount()
        {   
            // Return number of operations
            return operationCount;
        }
    }

}
