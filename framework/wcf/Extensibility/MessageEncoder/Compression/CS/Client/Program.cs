//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.GZipEncoder
{
    static class Program
    {
        static void Main()
        {
            SampleServerClient client = new SampleServerClient();

            Console.WriteLine("Calling Echo(string):");
            Console.WriteLine("Server responds: {0}", client.Echo("Simple hello"));

            Console.WriteLine();
            Console.WriteLine("Calling BigEcho(string[]):");
            string[] messages = new string[64];
            for (int i = 0; i < 64; i++)
            {
                messages[i] = "Hello " + i;
            }

            Console.WriteLine("Server responds: {0}", client.BigEcho(messages));

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
