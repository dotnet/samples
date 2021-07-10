
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.AddressHeaders
{
    // The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    // Client implementation code.
    class Client
    {
        public static readonly string IDName = "ID";
        public static readonly string IDNamespace = "http://Microsoft.Samples.AddressHeaders";

        static void Main()
        {
            // Create a client.
            HelloClient client = new HelloClient();

            // Add a reference-parameter header to the address.
            // Since the EndpointAddress class is immutable, we must use
            // EndpointAddressBuilder to change the value.
            EndpointAddressBuilder builder = new EndpointAddressBuilder(client.Endpoint.Address);
            AddressHeader header = AddressHeader.CreateAddressHeader(IDName, IDNamespace, "John");
            builder.Headers.Add(header);
            client.Endpoint.Address = builder.ToEndpointAddress();

            // Call the Hello service operation.
            Console.WriteLine(client.Hello());

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
