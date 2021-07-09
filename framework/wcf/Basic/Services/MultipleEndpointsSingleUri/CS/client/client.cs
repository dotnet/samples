
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.ServiceModel.Samples
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Note that the ListenUri must be communicated out-of-band.
            // That is, the metadata exposed by the service does not publish
            // the ListenUri, and thus the svcutil-generated config doesn't 
            // know about it.  

            // On the client, use ClientViaBehavior to specify 
            // the Uri where the server is listening.
            Uri via = new Uri("http://localhost/ServiceModelSamples/service.svc");

            // Create a client to talk to the Calculator contract
            CalculatorClient calcClient = new CalculatorClient();
            calcClient.ChannelFactory.Endpoint.Behaviors.Add(
                new ClientViaBehavior(via));
            double value1 = 100.00D;
            double value2 = 15.99D;
            double result = calcClient.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);
            calcClient.Close();

            // Create a client to talk to the Echo contract that is located
            // at the same EndpointAddress and ListenUri as the calculator contract
            EchoClient echoClient = new EchoClient("WSHttpBinding_IEcho");
            echoClient.ChannelFactory.Endpoint.Behaviors.Add(
                new ClientViaBehavior(via));
            Console.WriteLine(echoClient.Echo("Hello!"));
            echoClient.Close();

            // Create a client to talk to the Echo contract that is located
            // at a different EndpointAddress, but the same ListenUri
            EchoClient echoClient1 = new EchoClient("WSHttpBinding_IEcho1");
            echoClient1.ChannelFactory.Endpoint.Behaviors.Add(
                new ClientViaBehavior(via));
            Console.WriteLine(echoClient1.Echo("Hello!"));
            echoClient.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client application.");
            Console.ReadLine();
        }
    }
}


