//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.ServiceModel;

namespace Microsoft.Samples.ChunkingChannel
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = "image.jpg"; //replace with path of file to send
            string outputFilePath = "imageout.jpg"; //replace with path to use when saving received file
            Console.WriteLine("Press enter when service is available");
            Console.ReadLine();
            ChannelFactory<ITestService> factory = new ChannelFactory<ITestService>(
                new TcpChunkingBinding(),
                new EndpointAddress("net.tcp://localhost:9000/TestService/ep1"));
            ITestService service=factory.CreateChannel();
            bool success = false;
            try
            {
                FileStream infile = new FileStream(
                    inputFilePath,
                    FileMode.Open,
                    FileAccess.Read);
                Stream echo = service.EchoStream(infile);

                FileStream outfile = new FileStream(
                outputFilePath,
                FileMode.Create,
                FileAccess.Write);

                int count;
                byte[] buffer = new byte[4096];
                while ((count = echo.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outfile.Write(buffer, 0, count);
                    outfile.Flush();
                }
                infile.Close();
                echo.Close();
                outfile.Close();

                ((IClientChannel)service).Close();
                success = true;
            }
            finally
            {
                if (!success)
                {
                    ((IClientChannel)service).Abort();
                }
            }
        }
    }
}
