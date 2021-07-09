//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Samples.SBGenericsVTS.Invoker;
using Microsoft.Samples.SBGenericsVTS.SharedCode;

namespace Microsoft.Samples.SBGenericsVTS.Client40
{
    public class Program
    {
        static void Main(string[] args)
        {
            // First call
            ServerInvoker.Invoke<Customer>("tcp://127.0.0.1:1243/ListCustomerObject.rem", new List<Customer>(new Customer[] { new Customer("John", 23), new Customer("Mary", 45) }));

            // Second call
            ServerInvoker.Invoke<string>("tcp://127.0.0.1:1243/ListStringObject.rem", new List<string>(new string[] { "one", "two", "three" }));

            // Third call
            List<List<int>> listListInt = new List<List<int>>();
            listListInt.Add(new List<int>(new int[] { 1, 2 }));
            listListInt.Add(new List<int>(new int[] { 3, 4 }));
            ServerInvoker.Invoke<List<int>>("tcp://127.0.0.1:1243/ListListIntObject.rem", listListInt);

            // Fourth call
            List<int[]> listArrayInt = new List<int[]>();
            listArrayInt.Add(new int[] { 5, 6 });
            listArrayInt.Add(new int[] { 7, 8 });
            ServerInvoker.Invoke<int[]>("tcp://127.0.0.1:1243/ListArrayIntObject.rem", listArrayInt);

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
