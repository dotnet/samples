
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.ExpectedExceptions
{
    // The service contract is defined in generatedClient.cs, generated from the service
    // by the svcutil tool.
    //
    // When using such a client, applications must catch 

    // Client implementation code.
    class Client
    {
        static void Main()
        {
            DemonstrateCommunicationException();
            DemonstrateTimeoutException();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void DemonstrateCommunicationException()
        {
            // Create a client
            CalculatorClient client = new CalculatorClient();

            try
            {
                // Call the Add service operation.
                double value1 = 100.00D;
                double value2 = 15.99D;
                double result = client.Add(value1, value2);
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

                // Simulate a network problem by aborting the connection.
                Console.WriteLine("Simulated network problem occurs...");
                client.Abort();

                // Call the Divide service operation.  Now that the channel has been
                // abruptly terminated, the next call will fail.
                value1 = 22.00D;
                value2 = 7.00D;
                result = client.Divide(value1, value2);
                Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);

                // SHOULD NOT GET HERE -- Divide should throw

                // If we had gotten here, we would want to close the client gracefully so
                // that the channel closes gracefully and cleans up resources.
                client.Close();

                Console.WriteLine("Service successfully returned all results.");
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine("Got {0}", exception.GetType());
                client.Abort();
            }
            catch (CommunicationException exception)
            {
                // Control comes here when client.Divide throws.  The actual Exception
                // type is CommunicationObjectAbortedException, which is a subclass of
                // CommunicationException.
                Console.WriteLine("Got {0}", exception.GetType());
                client.Abort();
            }
        }

        static void DemonstrateTimeoutException()
        {
            // Create a client
            CalculatorClient client = new CalculatorClient();

            try
            {
                // Call the Add service operation.
                double value1 = 100.00D;
                double value2 = 15.99D;
                double result = client.Add(value1, value2);
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

                // Set a ridiculously small timeout.  This will cause the next call to
                // fail with a TimeoutException because it cannot process in time.
                Console.WriteLine("Set timeout too short for method to complete...");
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(0.001);

                // Call the Divide service operation.
                value1 = 22.00D;
                value2 = 7.00D;
                result = client.Divide(value1, value2);
                Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);

                // SHOULD NOT GET HERE -- Divide should throw

                // If we had gotten here, we would want to close the client gracefully so
                // that the channel closes gracefully and cleans up resources.
                client.Close();

                Console.WriteLine("Service successfully returned all results.");
            }
            catch (TimeoutException exception)
            {
                // Control comes here when client.Divide throws a TimeoutException.
                Console.WriteLine("Got {0}", exception.GetType());
                client.Abort();
            }
            catch (CommunicationException exception)
            {
                Console.WriteLine("Got {0}", exception.GetType());
                client.Abort();
            }
        }
    }
}
