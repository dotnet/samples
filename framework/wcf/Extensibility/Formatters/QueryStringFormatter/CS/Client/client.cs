
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    // We currently do not support client code generation using web service metadata for POX services.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client
            ChannelFactory<ICalculator> channelFactory = new ChannelFactory<ICalculator>("Calculator");

            // Apply query string formatter
            foreach (OperationDescription operationDescription in channelFactory.Endpoint.Contract.Operations)
            {
                EnableHttpGetRequestsBehavior.ReplaceFormatterBehavior(operationDescription, channelFactory.Endpoint.Address);
            }

            ICalculator client = channelFactory.CreateChannel();

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
            ((System.ServiceModel.Channels.IChannel)client).Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }

}
