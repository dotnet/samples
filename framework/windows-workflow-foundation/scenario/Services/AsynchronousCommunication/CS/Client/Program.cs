
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Client.ServiceReference1;

namespace Microsoft.Samples.HandyManService
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("When the services are ready, press <ENTER> to start the credit check.");
            Console.ReadLine();

            Console.WriteLine("Starting credit check...");
            Client.ServiceReference1.RentalApprovalServiceClient client = new Client.ServiceReference1.RentalApprovalServiceClient();
            SendData sendData = new SendData();
            sendData.credit = 600;
            sendData.value = 100000;
            if (client.SendData(sendData) == true)
            {
                Console.WriteLine("Credit approved!");
            }
            else
            {
                Console.WriteLine("Credit not approved...");
            }
            client.Close();
            client.ChannelFactory.Close();
            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
