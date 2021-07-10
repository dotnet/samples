//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.Mtom
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            byte[] binaryData = new byte[1000];
            MemoryStream stream = new MemoryStream(binaryData);

            // Upload a stream of 1000 bytes
            UploadClient client = new UploadClient();
            Console.WriteLine(client.Upload(stream));
            Console.WriteLine();
            stream.Close();

            // Compare the wire representations of messages with different payloads
            CompareMessageSize(100);
            CompareMessageSize(1000);
            CompareMessageSize(10000);
            CompareMessageSize(100000);
            CompareMessageSize(1000000);

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void CompareMessageSize(int dataSize)
        {
            // Create and buffer a message with a binary payload
            byte[] binaryData = new byte[dataSize];
            Message message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "action", binaryData);
            MessageBuffer buffer = message.CreateBufferedCopy(int.MaxValue);

            // Print the size of a text encoded copy
            int size = SizeOfTextMessage(buffer.CreateMessage());
            Console.WriteLine("Text encoding with a {0} byte payload: {1}", binaryData.Length, size);

            // Print the size of an MTOM encoded copy
            size = SizeOfMtomMessage(buffer.CreateMessage());
            Console.WriteLine("MTOM encoding with a {0} byte payload: {1}", binaryData.Length, size);

            Console.WriteLine();
            message.Close();
        }

        static int SizeOfTextMessage(Message message)
        {
            // Create a text encoder
            MessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
            MessageEncoderFactory factory = element.CreateMessageEncoderFactory();
            MessageEncoder encoder = factory.Encoder;

            // Write the message and return its length
            MemoryStream stream = new MemoryStream();
            encoder.WriteMessage(message, stream);
            int size = (int)stream.Length;
            
            message.Close();
            stream.Close();
            return size;
        }

        static int SizeOfMtomMessage(Message message)
        {
            // Create an MTOM encoder
            MessageEncodingBindingElement element = new MtomMessageEncodingBindingElement();
            MessageEncoderFactory factory = element.CreateMessageEncoderFactory();
            MessageEncoder encoder = factory.Encoder;

            // Write the message and return its length
            MemoryStream stream = new MemoryStream();
            encoder.WriteMessage(message, stream);
            int size = (int)stream.Length;
            
            stream.Close();
            message.Close();
            return size;
        }
    }
}
