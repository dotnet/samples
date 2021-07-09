
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Samples.Stream
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {

            string filePath = Path.Combine(
                System.Environment.CurrentDirectory,
                "clientfile");
            Console.WriteLine("Press <ENTER> when service is ready");
            Console.ReadLine();
            // Create a client with given client endpoint configuration

            StreamingSampleClient client1 = new StreamingSampleClient("BasicHttpBinding_IStreamingSample");

            Console.WriteLine("------ Using HTTP ------ ");

            Console.WriteLine("Calling GetStream()");
            System.IO.Stream stream1 = client1.GetStream("some dummy data");
            SaveStreamToFile(stream1, filePath);

            Console.WriteLine("Calling UploadStream()");
            FileStream instream1 = File.OpenRead(filePath);
            bool result1 = client1.UploadStream(instream1);

            instream1.Close();

            Console.WriteLine("Calling GetReversedStream()");
            stream1 = client1.GetReversedStream();
            SaveStreamToFile(stream1, filePath);
            client1.Close();

            //repeating using TCP
            StreamingSampleClient client2 = new StreamingSampleClient("CustomBinding_IStreamingSample");

            Console.WriteLine("------ Using Custom HTTP ------ ");

            Console.WriteLine("Calling GetStream()");
            System.IO.Stream stream2 = client2.GetStream("some dummy data");
            SaveStreamToFile(stream2, filePath);

            Console.WriteLine("Calling UploadStream()");
            FileStream instream2 = File.OpenRead(filePath);
            bool result2 = client2.UploadStream(instream2);

            instream2.Close();

            Console.WriteLine("Calling GetReversedStream()");
            stream2 = client2.GetReversedStream();
            SaveStreamToFile(stream2, filePath);
            client2.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
        
        static void SaveStreamToFile(System.IO.Stream stream, string filePath)
        {
            Console.WriteLine("Saving to file {0}", filePath);
            FileStream outstream = File.Open(filePath, FileMode.Create, FileAccess.Write);
            CopyStream(stream, outstream);
            outstream.Close();
            stream.Close();
            Console.WriteLine();
            Console.WriteLine("File {0} saved", filePath);
        }

        static void CopyStream(System.IO.Stream instream, System.IO.Stream outstream)
        {
            //read from the input stream in 4K chunks
            //and save to output stream
            const int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int count = 0;
            int bytecount = 0;
            while ((count = instream.Read(buffer, 0, bufferLen)) > 0)
            {

                outstream.Write(buffer, 0, count);
                Console.Write(".");
                bytecount += count;
            }
            Console.WriteLine();
            Console.WriteLine("Wrote {0} bytes to stream", bytecount);
        }
    }
}
