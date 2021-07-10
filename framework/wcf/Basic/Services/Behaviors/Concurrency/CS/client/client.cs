
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace Microsoft.Samples.Concurrency
{
    // The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    // Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client
            CalculatorConcurrencyClient client = new CalculatorConcurrencyClient();

            Console.WriteLine("Press <ENTER> to terminate client once the output is displayed.");
            Console.WriteLine();

            // Communicate with service using asynchronous methods.
            client.BeginGetConcurrencyMode(GetConcurrencyModeCallback, client);

            // BeginAdd
            double value1 = 100.00D;
            double value2 = 15.99D;
            IAsyncResult arAdd = client.BeginAdd(value1, value2, AddCallback, client);
            Console.WriteLine("Add({0},{1})", value1, value2);

            // BeginSubtract
            value1 = 145.00D;
            value2 = 76.54D;
            IAsyncResult arSubtract = client.BeginSubtract(value1, value2, SubtractCallback, client);
            Console.WriteLine("Subtract({0},{1})", value1, value2);

            // BeginMultiply
            value1 = 9.00D;
            value2 = 81.25D;
            IAsyncResult arMultiply = client.BeginMultiply(value1, value2, MultiplyCallback, client);
            Console.WriteLine("Multiply({0},{1})", value1, value2);

            // BeginDivide
            value1 = 22.00D;
            value2 = 7.00D;
            IAsyncResult arDivide = client.BeginDivide(value1, value2, DivideCallback, client);
            Console.WriteLine("Divide({0},{1})", value1, value2);

            client.BeginGetOperationCount(GetOperationCountCallback, client);

            Console.ReadLine();

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }

        // Asynchronous callbacks for displaying results.
        static void GetConcurrencyModeCallback(IAsyncResult ar)
        {
            string result = ((CalculatorConcurrencyClient)ar.AsyncState).EndGetConcurrencyMode(ar);
            Console.WriteLine("ConcurrencyMode : {0}", result);
        }

        static void GetOperationCountCallback(IAsyncResult ar)
        {
            int result = ((CalculatorConcurrencyClient)ar.AsyncState).EndGetOperationCount(ar);
            Console.WriteLine("OperationCount : {0}", result);
        }

        static void AddCallback(IAsyncResult ar)
        {
            double result = ((CalculatorConcurrencyClient)ar.AsyncState).EndAdd(ar);
            Console.WriteLine("Add Result : " + result);
        }

        static void SubtractCallback(IAsyncResult ar)
        {
            double result = ((CalculatorConcurrencyClient)ar.AsyncState).EndSubtract(ar);
            Console.WriteLine("Subtract Result : " + result);
        }

        static void MultiplyCallback(IAsyncResult ar)
        {
            double result = ((CalculatorConcurrencyClient)ar.AsyncState).EndMultiply(ar);
            Console.WriteLine("Multiply Result : " + result);
        }

        static void DivideCallback(IAsyncResult ar)
        {
            double result = ((CalculatorConcurrencyClient)ar.AsyncState).EndDivide(ar);
            Console.WriteLine("Divide Result : " + result);
        }
    }
}
