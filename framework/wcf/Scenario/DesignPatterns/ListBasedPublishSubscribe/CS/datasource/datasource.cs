//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.

    class Client : ISampleContractCallback
    {
        static void Main(string[] args)
        {
            InstanceContext site = new InstanceContext(new Client());
            SampleContractClient client = new SampleContractClient(site);

            Console.WriteLine("Sending PublishPriceChange(Gold, 400.00D, -0.25D)");
            client.PublishPriceChange("Gold", 400.00D, -0.25D);

            Console.WriteLine("Sending PublishPriceChange(Silver, 7.00D, -0.20D)");
            client.PublishPriceChange("Silver", 7.00D, -0.20D);

            Console.WriteLine("Sending PublishPriceChange(Platinum, 850.00D, +0.50D)");
            client.PublishPriceChange("Platinum", 850.00D, +0.50D);

            Console.WriteLine("Sending PublishPriceChange(Gold, 401.00D, 1.00D)");
            client.PublishPriceChange("Gold", 401.00D, 1.00D);

            Console.WriteLine("Sending PublishPriceChange(Silver, 6.60D, -0.40D)");
            client.PublishPriceChange("Silver", 6.60D, -0.40D);

            Console.WriteLine();
            Console.WriteLine("Press ENTER to shut down data source");
            Console.ReadLine();

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }

        public void PriceChange(string item, double price, double change)
        {
            Console.WriteLine("PriceChange(item {0}, price {1}, change {2})", item, price.ToString("C"), change);
        }

    }
}

