
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client to endpoint configuration for ICalculator
            CalculatorClient client = new CalculatorClient();
            Console.WriteLine("Communicate with default ICalculator endpoint.");
            // call operations
            DoCalculations(client);

            //close client and release resources
            client.Close();

            //Create a client to endpoint configuration for ICalculatorSession
            CalculatorSessionClient sClient = new CalculatorSessionClient();

            Console.WriteLine("Communicate with ICalculatorSession endpoint.");
            sClient.Clear();
            sClient.AddTo(100.0D);
            sClient.SubtractFrom(50.0D);
            sClient.MultiplyBy(17.65D);
            sClient.DivideBy(2.0D);
            double result = sClient.Result();
            Console.WriteLine("0, + 100, - 50, * 17.65, / 2 = {0}", result);

            //close client and release resources
            sClient.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void DoCalculations(CalculatorClient client)
        {

            // Call the Add service operation.
            double value1 = 100.00D;
            double value2 = 15.99D;
            double result = client.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation.
            value1 = 145.00D;
            value2 = 76.54D;
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            value1 = 9.00D;
            value2 = 81.25D;
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            value1 = 22.00D;
            value2 = 7.00D;
            result = client.Divide(value1, value2);
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
        }

    }
}

