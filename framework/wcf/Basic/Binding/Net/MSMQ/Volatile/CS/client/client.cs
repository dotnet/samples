//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.MSMQVolatileSample
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client with
            Random r = new Random(137);

            StockTickerClient client = new StockTickerClient();

            float price = 43.23F;
            for (int i = 0; i < 10; i++)
            {
                float increment = 0.01f * (r.Next(10));
                client.StockTick("zzz" + i, price + increment);
            }

            //Closing the client gracefully cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}

