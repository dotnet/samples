
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Text;

namespace Microsoft.Samples.Untyped
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client
            CalculatorClient client = new CalculatorClient();
            String RequestAction = "http://test/Message_RequestAction";
            using (new OperationContextScope(client.InnerChannel))
            {
                // Call the Sum service operation.
                int[] values = { 1, 2, 3, 4, 5 };
                Message request = Message.CreateMessage(OperationContext.Current.OutgoingMessageHeaders.MessageVersion, RequestAction, values);
                Message reply = client.ComputeSum(request);
                int response = reply.GetBody<int>();

                Console.WriteLine("Sum of numbers passed (1,2,3,4,5) = {0}", response);
            }

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
