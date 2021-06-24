using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    class client
    {
        static void Main()
        {
            string input;

            ShoppingCartClient client = new ShoppingCartClient();
            
            Console.Write("Enter the name of the product: ");
            input = Console.ReadLine();
            client.AddItem(input);

            Console.Write("Enter the name of the product: ");
            input = Console.ReadLine();
            client.AddItem(input);

            PrintCart(client);

            client.Close();

            Console.WriteLine("Press ENTER to shut down client");
            Console.ReadLine();
        }

        static void PrintCart(ShoppingCartClient client)
        {
            Console.WriteLine();            
            Console.WriteLine("Shopping cart currently contains the following items.");

            string[] items = client.GetCart();

            foreach (string book in items)
            {
                Console.WriteLine(book);
            }            
        }
    }
}

