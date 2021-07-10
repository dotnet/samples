
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Threading;

namespace Microsoft.Samples.Instancing
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.Instancing", SessionMode=SessionMode.Required)]
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

    // Define a service contract to inspect instance state
    [ServiceContract(Namespace = "http://Microsoft.Samples.Instancing", SessionMode = SessionMode.Required)]
    public interface ICalculatorInstance : ICalculator
    {
        [OperationContract]
        string GetInstanceContextMode();
        [OperationContract]
        int GetInstanceId();
        [OperationContract]
        int GetOperationCount();
    }

    // Enable one of the following instance modes to compare instancing behaviors.
    
    // PerSession creates an instance per channel session
    // This requires a binding that supports session
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]

    // PerCall creates a new instance for each operation
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]

    // Singleton creates a single instance for application lifetime
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

    public class CalculatorService : ICalculatorInstance
    {
        static Object syncObject = new object();
        static int instanceCount;
        int instanceId;
        int operationCount;

        public CalculatorService()
        {
            lock (syncObject)
            {
                instanceCount++;
                instanceId = instanceCount;
            }
        }

        public double Add(double n1, double n2)
        {
            operationCount++;
            return n1 + n2;
        }

        public double Subtract(double n1, double n2)
        {
            Interlocked.Increment(ref operationCount);
            return n1 - n2;
        }

        public double Multiply(double n1, double n2)
        {
            Interlocked.Increment(ref operationCount);
            return n1 * n2;
        }

        public double Divide(double n1, double n2)
        {
            Interlocked.Increment(ref operationCount);
            return n1 / n2;
        }

        public string GetInstanceContextMode()
        {   // Return the InstanceContextMode of the service
            ServiceHost host = (ServiceHost)OperationContext.Current.Host;
            ServiceBehaviorAttribute behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            return behavior.InstanceContextMode.ToString();
        }

        public int GetInstanceId()
        {   // Return the id for this instance
            return instanceId;
        }

        public int GetOperationCount()
        {   // Return the number of ICalculator operations performed on this instance
            lock (syncObject)
            {
                return operationCount;
            }
        }

    }

}
