//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Transactions;

namespace Microsoft.ServiceModel.Samples
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client using either wsat or oletx endpoint configurations
            CalculatorClient client = new CalculatorClient("WSAtomicTransaction_endpoint");
            // CalculatorClient client = new CalculatorClient("OleTransactions_endpoint");

            // Start a transaction scope
            using (TransactionScope tx =
                        new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                Console.WriteLine("Starting transaction");

                // Call the Add service operation
                //  - generatedClient will flow the required active transaction
                double value1 = 100.00D;
                double value2 = 15.99D;
                double result = client.Add(value1, value2);
                Console.WriteLine("  Add({0},{1}) = {2}", value1, value2, result);

                // Call the Subtract service operation
                //  - generatedClient will flow the allowed active transaction
                value1 = 145.00D;
                value2 = 76.54D;
                result = client.Subtract(value1, value2);
                Console.WriteLine("  Subtract({0},{1}) = {2}", value1, value2, result);

                // Start a transaction scope that suppresses the current transaction
                using (TransactionScope txSuppress =
                            new TransactionScope(TransactionScopeOption.Suppress))
                {
                    // Call the Subtract service operation
                    //  - the active transaction is suppressed from the generatedClient
                    //    and no transaction will flow
                    value1 = 21.05D;
                    value2 = 42.16D;
                    result = client.Subtract(value1, value2);
                    Console.WriteLine("  Subtract({0},{1}) = {2}", value1, value2, result);

                    // Complete the suppressed scope
                    txSuppress.Complete();
                }

                // Call the Multiply service operation
                // - generatedClient will not flow the active transaction
                value1 = 9.00D;
                value2 = 81.25D;
                result = client.Multiply(value1, value2);
                Console.WriteLine("  Multiply({0},{1}) = {2}", value1, value2, result);

                // Call the Divide service operation.
                // - generatedClient will not flow the active transaction
                value1 = 22.00D;
                value2 = 7.00D;
                result = client.Divide(value1, value2);
                Console.WriteLine("  Divide({0},{1}) = {2}", value1, value2, result);

                // Complete the transaction scope
                Console.WriteLine("  Completing transaction");
                tx.Complete();
            }

            Console.WriteLine("Transaction committed");

            // Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
