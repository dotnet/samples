
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Configuration;
using System.ServiceModel;
using System.Xml;

namespace Microsoft.Samples.RetrieveMetadata
{

    // Client implementation code.
    // Dynamically retrieve metadata from the service and use the metadata to 
    // communicate with each endpoint that implements the ICalculator contract. 
    class Client
    {
        static void Main()
        {
            // Create a MetadataExchangeClient for retrieving metadata.
            EndpointAddress mexAddress = new EndpointAddress(ConfigurationManager.AppSettings["mexAddress"]);
            MetadataExchangeClient mexClient = new MetadataExchangeClient(mexAddress);

            // Retrieve the metadata for all endpoints using metadata exchange protocol (mex).
            MetadataSet metadataSet = mexClient.GetMetadata();

            //Convert the metadata into endpoints
            WsdlImporter importer = new WsdlImporter(metadataSet);
            ServiceEndpointCollection endpoints = importer.ImportAllEndpoints();

            CalculatorClient client = null;
            ContractDescription contract = ContractDescription.GetContract(typeof(ICalculator));
            // Communicate with each endpoint that supports the ICalculator contract.
            foreach (ServiceEndpoint ep in endpoints)
            {
                if (ep.Contract.Namespace.Equals(contract.Namespace) && ep.Contract.Name.Equals(contract.Name))
                {
                    // Create a client using the endpoint address and binding.
                    client = new CalculatorClient(ep.Binding, new EndpointAddress(ep.Address.Uri));
                    Console.WriteLine("Communicate with endpoint: ");
                    Console.WriteLine("   AddressPath={0}", ep.Address.Uri.PathAndQuery);
                    Console.WriteLine("   Binding={0}", ep.Binding.Name);
                    // call operations
                    DoCalculations(client);

                    //Closing the client gracefully closes the connection and cleans up resources
                    client.Close();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void DoCalculations(CalculatorClient client)
        {
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
        }
    }
}
