
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Data
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client
            DataContractCalculatorClient client = new DataContractCalculatorClient();

            // Call the Add service operation.
            ComplexNumber value1 = new ComplexNumber(); value1.Real = 1; value1.Imaginary = 2;
            ComplexNumber value2 = new ComplexNumber(); value2.Real = 3; value2.Imaginary = 4;
            ComplexNumber result = client.Add(value1, value2);
            Console.WriteLine("Add({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);

            // Call the Subtract service operation.
            value1 = new ComplexNumber(); value1.Real = 1; value1.Imaginary = 2;
            value2 = new ComplexNumber(); value2.Real = 3; value2.Imaginary = 4;
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);

            // Call the Multiply service operation.
            value1 = new ComplexNumber(); value1.Real = 2; value1.Imaginary = 3;
            value2 = new ComplexNumber(); value2.Real = 4; value2.Imaginary = 7;
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);

            // Call the Divide service operation.
            value1 = new ComplexNumber(); value1.Real = 3; value1.Imaginary = 7;
            value2 = new ComplexNumber(); value2.Real = 5; value2.Imaginary = -2;
            result = client.Divide(value1, value2);
            Console.WriteLine("Divide({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
