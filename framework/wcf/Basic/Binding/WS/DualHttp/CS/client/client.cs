//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.DualHttp
{
    // The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    // Define class which implements callback interface of duplex contract
    public class CallbackHandler : ICalculatorDuplexCallback
    {
        public void Result(double result)
        {
            Console.WriteLine("Result({0})", result);
        }

        public void Equation(string eqn)
        {
            Console.WriteLine("Equation({0})", eqn);
        }
    }

    class Client
    {
        static void Main()
        {
            // Construct InstanceContext to handle messages on callback interface
            InstanceContext instanceContext = new InstanceContext(new CallbackHandler());

            // Create a client with given client endpoint configuration
            CalculatorDuplexClient client = new CalculatorDuplexClient(instanceContext);

            Console.WriteLine("Press <ENTER> to terminate client once the output is displayed.");
            Console.WriteLine();

            // Call the AddTo service operation.
            double value = 100.00D;
            client.AddTo(value);

            // Call the SubtractFrom service operation.
            value = 50.00D;
            client.SubtractFrom(value);

            // Call the MultiplyBy service operation.
            value = 17.65D;
            client.MultiplyBy(value);

            // Call the DivideBy service operation.
            value = 2.00D;
            client.DivideBy(value);

            // Complete equation
            client.Clear();

            Console.ReadLine();

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }
    }
}
