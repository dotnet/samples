//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Samples.HttpCookieSession
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use simple binding which allows HttpCookies
            Console.WriteLine("Simple binding:");
            ShoppingCartClient client = new ShoppingCartClient("AllowCookies");
            Shop(client);
            client.Close();
            
            // Use smart binding which explicitly terminates session
            Console.WriteLine("Smart binding:");
            ShoppingCartClient client2 = new ShoppingCartClient("CookieSession");
            Shop(client2);
            client2.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void Shop(ShoppingCartClient client)
        {
            int count = client.AddItem(10000, 2);
            Console.WriteLine("AddItem({0},{1}): ItemCount={2}", 10000, 2, count);
            count = client.AddItem(10550, 5);
            Console.WriteLine("AddItem({0},{1}): ItemCount={2}", 10550, 5, count);
            count = client.RemoveItem(10550, 2);
            Console.WriteLine("RemoveItem({0},{1}): ItemCount={2}", 10550, 2, count);

            Dictionary<int,int> items = client.GetItems();
            Console.WriteLine("Items");
            foreach (KeyValuePair<int, int> item in items)
            {
                Console.WriteLine("{0}, {1}", item.Key, item.Value);
            }
        }
    }
}
