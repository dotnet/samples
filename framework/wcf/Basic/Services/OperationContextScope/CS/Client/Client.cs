//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    //Utility class that acts as the header that client will add to list of incoming headers
    public static class CustomHeader
    {
        public static readonly String HeaderName = "MessageHeaderGUID";
        public static readonly String HeaderNamespace = "http://Microsoft.ServiceModel.Samples/GUID";
    }

    class MessageHeaderClient
    {
        static void Main(string[] args)
        {
            //Create two clients to the remote service.
            MessageHeaderReaderClient client1 = new MessageHeaderReaderClient();
            MessageHeaderReaderClient client2 = new MessageHeaderReaderClient();


            //Create an OperationContextScope with client1 so we can add headers.
            using (new OperationContextScope(client1.InnerChannel))
            {
                //Create a new GUID that we will send as header.
                String guid = Guid.NewGuid().ToString();

                //Create a MessageHeader for the guid we just created.
                MessageHeader customHeader = MessageHeader.CreateHeader(CustomHeader.HeaderName, CustomHeader.HeaderNamespace, guid);

                //Add the header to the OutgoingMessageHeader collection.
                OperationContext.Current.OutgoingMessageHeaders.Add(customHeader);

                //Now call RetreieveHeader on both the proxies. Since the OperationContextScope is tied to 
                //client1's InnerChannel, the header should only be added to calls made on that client.
                //Calls made on client2 should not be sending the header across even though the call
                //is made in the same OperationContextScope.
                Console.WriteLine("Using client1 to send message");
                Console.WriteLine("Did server retrieve the header? : Actual: {0}, Expected: True", client1.RetrieveHeader(guid));

                Console.WriteLine();
                Console.WriteLine("Using client2 to send message");
                Console.WriteLine("Did server retrieve the header? : Actual: {0}, Expected: False", client2.RetrieveHeader(guid));
            }

            //Close the proxies.
            client1.Close();
            client2.Close();
        
            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}

