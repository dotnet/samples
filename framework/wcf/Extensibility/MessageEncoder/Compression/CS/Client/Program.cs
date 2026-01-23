//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.GZipEncoder
{
    static class Program
    {
        static void Main()
        {
#if NET6_0_OR_GREATER
            Binding binding = new CustomBinding(new GZipMessageEncodingBindingElement(), new HttpTransportBindingElement());
            EndpointAddress endpointAddress = new EndpointAddress(new Uri("http://localhost:8000/samples/GZipEncoder"));
            SampleServerClient client = new SampleServerClient(binding, endpointAddress);
#else
            SampleServerClient client = new SampleServerClient();
#endif


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
