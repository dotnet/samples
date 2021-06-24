//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

using System.Security.Permissions;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace Microsoft.Samples.DurableIssuedTokenProvider
{
	class Client
	{
        static void Main(string[] args)
        {
            // File based cache for issued tokens
            IssuedTokenCache cache = new FileIssuedTokenCache("cache.xml");

            // If you want only in-memory caching, replace the above by the following line
            // IssuedTokenCache cache = new InMemoryIssuedTokenCache();

            for (int i = 0; i < 5; ++i)
            {
                ChannelFactory<ICalculator> clientFactory = new ChannelFactory<ICalculator>("ServiceFed");
                clientFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
                DurableIssuedTokenClientCredentials durableCreds = new DurableIssuedTokenClientCredentials();
                durableCreds.IssuedTokenCache = cache;
                durableCreds.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;
                clientFactory.Endpoint.Behaviors.Add(durableCreds);

                ICalculator client = clientFactory.CreateChannel();

                // Call the Add service operation.
                double value1 = 100.00D;
                double value2 = 15.99D;
                double result = client.Add(value1, value2);
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

                // Call the Subtract service operation.
                value1 = 145.00D;
                value2 = 76.54D;
                result = client.Subtract(value1, value2);
                Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

                // Call the Multiply service operation.
                value1 = 9.00D;
                value2 = 81.25D;
                result = client.Multiply(value1, value2);
                Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

                // Call the Divide service operation.
                value1 = 22.00D;
                value2 = 7.00D;
                result = client.Divide(value1, value2);
                Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);

                //Closing the client gracefully closes the connection and cleans up resources
                ((IChannel)client).Close();
                clientFactory.Close();
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
	}
}

