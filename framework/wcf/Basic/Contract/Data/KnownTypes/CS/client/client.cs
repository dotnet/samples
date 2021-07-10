
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.KnownTypes
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
            ComplexNumber value1 = new ComplexNumber(); value1.real = 1; value1.imaginary = 2;
            ComplexNumber value2 = new ComplexNumber(); value2.real = 3; value2.imaginary = 4;
            ComplexNumber result = client.Add(value1, value2);
            Console.WriteLine("Add({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.real, value1.imaginary, value2.real, value2.imaginary, result.real, result.imaginary);
            if (result is ComplexNumberWithMagnitude)
            {
                Console.WriteLine("Magnitude: {0}", ((ComplexNumberWithMagnitude)result).Magnitude);
            }
            else
            {
                Console.WriteLine("No magnitude was sent from the service");
            }


            // Call the Subtract service operation.
            value1 = new ComplexNumber(); value1.real = 1; value1.imaginary = 2;
            value2 = new ComplexNumber(); value2.real = 3; value2.imaginary = 4;
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.real, value1.imaginary, value2.real, value2.imaginary, result.real, result.imaginary);
            if (result is ComplexNumberWithMagnitude)
            {
                Console.WriteLine("Magnitude: {0}", ((ComplexNumberWithMagnitude)result).Magnitude);
            }
            else
            {
                Console.WriteLine("No magnitude was sent from the service");
            }

            // Call the Multiply service operation.
            value1 = new ComplexNumber(); value1.real = 2; value1.imaginary = 3;
            value2 = new ComplexNumber(); value2.real = 4; value2.imaginary = 7;
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.real, value1.imaginary, value2.real, value2.imaginary, result.real, result.imaginary);
            if (result is ComplexNumberWithMagnitude)
            {
                Console.WriteLine("Magnitude: {0}", ((ComplexNumberWithMagnitude)result).Magnitude);
            }
            else
            {
                Console.WriteLine("No magnitude was sent from the service");
            }

            // Call the Divide service operation.
            value1 = new ComplexNumber(); value1.real = 3; value1.imaginary = 7;
            value2 = new ComplexNumber(); value2.real = 5; value2.imaginary = -2;
            result = client.Divide(value1, value2);
            Console.WriteLine("Divide({0} + {1}i, {2} + {3}i) = {4} + {5}i",
                value1.real, value1.imaginary, value2.real, value2.imaginary, result.real, result.imaginary);
            if (result is ComplexNumberWithMagnitude)
            {
                Console.WriteLine("Magnitude: {0}", ((ComplexNumberWithMagnitude)result).Magnitude);
            }
            else
            {
                Console.WriteLine("No magnitude was sent from the service");
            }

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
