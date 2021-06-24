' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports System.Transactions

Namespace Microsoft.ServiceModel.Samples

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            ' Create a client using either wsat or oletx endpoint configurations
            Dim client As New CalculatorClient("WSAtomicTransaction_endpoint")
            'Dim client As New CalculatorClient("OleTransactions_endpoint")

            ' Start a transaction scope
            Using tx As New TransactionScope(TransactionScopeOption.RequiresNew)

                Console.WriteLine("Starting transaction")

                ' Call the Add service operation
                '  - generatedClient will flow the required active transaction
                Dim value1 As Double = 100
                Dim value2 As Double = 15.99
                Dim result As Double = client.Add(value1, value2)
                Console.WriteLine("  Add({0},{1}) = {2}", value1, value2, result)

                ' Call the Subtract service operation
                '  - generatedClient will flow the allowed active transaction
                value1 = 145
                value2 = 76.54
                result = client.Subtract(value1, value2)
                Console.WriteLine("  Subtract({0},{1}) = {2}", value1, value2, result)

                ' Start a transaction scope that suppresses the current transaction
                Using txSuppress As New TransactionScope(TransactionScopeOption.Suppress)

                    ' Call the Subtract service operation
                    '  - the active transaction is suppressed from the generatedClient
                    '    and no transaction will flow
                    value1 = 21.05
                    value2 = 42.16
                    result = client.Subtract(value1, value2)
                    Console.WriteLine("  Subtract({0},{1}) = {2}", value1, value2, result)

                    ' Complete the suppressed scope
                    txSuppress.Complete()

                End Using

                ' Call the Multiply service operation
                ' - generatedClient will not flow the active transaction
                value1 = 9
                value2 = 81.25
                result = client.Multiply(value1, value2)
                Console.WriteLine("  Multiply({0},{1}) = {2}", value1, value2, result)

                ' Call the Divide service operation.
                ' - generatedClient will not flow the active transaction
                value1 = 22
                value2 = 7
                result = client.Divide(value1, value2)
                Console.WriteLine("  Divide({0},{1}) = {2}", value1, value2, result)

                ' Complete the transaction scope
                Console.WriteLine("  Completing transaction")
                tx.Complete()

            End Using

            Console.WriteLine("Transaction committed")

            ' Closing the client gracefully closes the connection and cleans up resources
            client.Close()

            Console.WriteLine("Press <ENTER> to terminate client.")
            Console.ReadLine()

        End Sub

    End Class

End Namespace
