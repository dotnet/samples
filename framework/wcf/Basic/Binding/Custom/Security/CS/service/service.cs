
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Security
{
    // Define a duplex service contract.
    // A duplex contract consists of two interfaces.
    // The primary interface is used to send messages from client to service.
    // The callback interface is used to send messages from service back to client.
    // ICalculatorDuplex allows one to perform multiple operations on a running result.
    // The result is sent back after each operation on the ICalculatorCallback interface.
    [ServiceContract(Namespace = "http://Microsoft.Samples.Security", SessionMode = SessionMode.Required,
                     CallbackContract = typeof(ICalculatorDuplexCallback))]
    public interface ICalculatorDuplex
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
    }

    // The callback interface is used to send messages from service back to client.
    // The Result operation will return the current result after each operation.
    // The Equation opertion will return the complete equation after Clear() is called.
    public interface ICalculatorDuplexCallback
    {
        [OperationContract(IsOneWay = true)]
        void Result(double result);
        [OperationContract(IsOneWay = true)]
        void Equation(string eqn);
    }

    // Service class which implements a duplex service contract.
    // Use an InstanceContextMode of PrivateSession to store the result
    // An instance of the service will be bound to each duplex session
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class CalculatorService : ICalculatorDuplex
    {
        double result;
        string equation;
        ICalculatorDuplexCallback callback = null;

        public CalculatorService()
        {
            result = 0.0D;
            equation = result.ToString();
            callback = OperationContext.Current.GetCallbackChannel<ICalculatorDuplexCallback>();
        }

        public void Clear()
        {
            callback.Equation(equation + " = " + result.ToString());
            result = 0.0D;
            equation = result.ToString();
        }

        public void AddTo(double n)
        {
            result += n;
            equation += " + " + n.ToString();
            callback.Result(result);
        }

        public void SubtractFrom(double n)
        {
            result -= n;
            equation += " - " + n.ToString();
            callback.Result(result);
        }

        public void MultiplyBy(double n)
        {
            result *= n;
            equation += " * " + n.ToString();
            callback.Result(result);
        }

        public void DivideBy(double n)
        {
            result /= n;
            equation += " / " + n.ToString();
            callback.Result(result);
        }

        static void Main()
        {
            // Create a ServiceHost for the CalculatorService type
            using (ServiceHost host = new ServiceHost(typeof(CalculatorService)))
            {
                // Open the ServiceHostBase to create listeners and start listening for messages.
                host.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}

