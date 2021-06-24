//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

namespace Microsoft.Samples.UserNamePasswordValidator
{
    //The service contract is defined in generatedProxy.cs, generated from the service by the svcutil tool.
    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Get the username and password
            Console.WriteLine("Username authentication required.");
            Console.WriteLine("Provide a username.");
            Console.WriteLine("   Enter username: (test1)");
            string username = Console.ReadLine();
            Console.WriteLine("   Enter password:");
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    if (info.KeyChar != '\0')
                    {
                        password += info.KeyChar;
                    }
                    info = Console.ReadKey(true);
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (password != "")
                    {
                        password = password.Substring(0, password.Length - 1);

                    }
                    info = Console.ReadKey(true);
                }
            }

            for (int i = 0; i < password.Length; i++)
            {
                Console.Write("*");
            }
            Console.WriteLine();

            // Create a proxy with Username endpoint configuration
            CalculatorProxy proxy = new CalculatorProxy("Username");

            try
            {
                proxy.ClientCredentials.UserName.UserName = username;
                proxy.ClientCredentials.UserName.Password = password;

                // Call the Add service operation.
                double value1 = 100.00D;
                double value2 = 15.99D;
                double result = proxy.Add(value1, value2);
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

                // Call the Subtract service operation.
                value1 = 145.00D;
                value2 = 76.54D;
                result = proxy.Subtract(value1, value2);
                Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

                // Call the Multiply service operation.
                value1 = 9.00D;
                value2 = 81.25D;
                result = proxy.Multiply(value1, value2);
                Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

                // Call the Divide service operation.
                value1 = 22.00D;
                value2 = 7.00D;
                result = proxy.Divide(value1, value2);
                Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
                proxy.Close();
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("Call timed out : {0}", e.Message);
                proxy.Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine("Call failed:");
                while (e != null)
                {
                    Console.WriteLine("\t{0}", e.Message);
                    e = e.InnerException;
                }
                proxy.Abort();
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}

