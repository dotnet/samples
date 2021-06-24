
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Session
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client with given client endpoint configuration
            CalculatorSessionClient client = new CalculatorSessionClient();

            client.Clear();
            client.AddTo(100.0D);
            client.SubtractFrom(50.0D);
            client.MultiplyBy(17.65D);
            client.DivideBy(2.0D);
            double result = client.Result();
            Console.WriteLine("(((0 + 100) - 50) * 17.65) / 2 = {0}", result);

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
