
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    // The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool

    // Client implementation code
    class Client
    {
        static void Main()
        {
            // Create a client with given client endpoint configuration
            ErrorCalculatorClient client = new ErrorCalculatorClient();

            // Call the Add service operation
            int value1 = 15;
            int value2 = 3;
            int result = client.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation
            value1 = 145;
            value2 = 76;
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation
            value1 = 9;
            value2 = 81;
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            try
            {
                Console.WriteLine("Forcing an error in Divide");
                // Call the Divide service operation - trigger a divide by zero error
                value1 = 22;
                value2 = 0;
                result = client.Divide(value1, value2);
                Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
            }
            catch (FaultException e)
            {
                Console.WriteLine("FaultException: " + e.GetType().Name + " - " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.GetType().Name + " - " + e.Message);
            }

            try
            {
                // Call the Factorial service operation - trigger an overflow error
                Console.WriteLine("Forcing an error in Factorial");
                value1 = 0;
                result = client.Factorial(value1);
                Console.WriteLine("Factorial({0}) = {1}", value1, result);
            }
            catch (FaultException e)
            {
                Console.WriteLine("FaultException: " + e.GetType().Name + " - " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.GetType().Name + " - " + e.Message);
            }

            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
