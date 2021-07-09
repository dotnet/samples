//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Web;

namespace Microsoft.Samples.BasicHttpService
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8000");
            Console.WriteLine("Service is hosted at: " + baseAddress.AbsoluteUri);
            Console.WriteLine("Service help page is at: " + baseAddress.AbsoluteUri + "help");

            using (WebServiceHost host = new WebServiceHost(typeof(Service), baseAddress))
            {
                //WebServiceHost will automatically create a default endpoint at the base address using the 
                //WebHttpBinding and the WebHttpBehavior, so there's no need to set that up explicitly
                host.Open();

                using (WebChannelFactory<IService> cf = new WebChannelFactory<IService>(baseAddress))
                {
                    IService channel = cf.CreateChannel();

                    string s;

                    Console.WriteLine("Calling EchoWithGet via HTTP GET: ");
                    s = channel.EchoWithGet("Hello, world");
                    Console.WriteLine("   Output: {0}", s);

                    Console.WriteLine("");
                    Console.WriteLine("This can also be accomplished by navigating to");
                    Console.WriteLine("http://localhost:8000/EchoWithGet?s=Hello, world!");
                    Console.WriteLine("in a Web browser while this sample is running.");

                    Console.WriteLine("");

                    Console.WriteLine("Calling EchoWithPost via HTTP POST: ");
                    s = channel.EchoWithPost("Hello, world");
                    Console.WriteLine("   Output: {0}", s);

                    Console.WriteLine("");
                }


                Console.WriteLine("Press any key to terminate");
                Console.ReadLine();
            }
        }
    }
}
