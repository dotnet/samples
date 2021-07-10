//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.ChunkingChannel
{
    class Host
    {
        public static void Main()
        {
            ServiceHost host = new ServiceHost(typeof(service), 
                new Uri("net.tcp://localhost:9000/TestService"));
            host.AddServiceEndpoint(
                typeof(ITestService),
                new TcpChunkingBinding(),
                "ep1");
            host.Open();
            Console.WriteLine("Service started, press enter to exit");
            Console.ReadLine();
            host.Close();
        }
    }
}
