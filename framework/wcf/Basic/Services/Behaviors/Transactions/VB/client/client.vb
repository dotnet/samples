' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports System.Transactions

Namespace Microsoft.ServiceModel.Samples

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            ' Create a client
            Dim client As New CalculatorClient()

            ' Create a transaction scope with the default isolation level of Serializable
            Using tx As New TransactionScope()

                Console.WriteLine("Starting transaction")

                ' Call the Add service operation.
                Dim value As Double = 100
                Console.WriteLine("  Adding {0}, running total={1}", value, client.Add(value))

                ' Call the Subtract service operation.
                value = 45
                Console.WriteLine("  Subtracting {0}, running total={1}", value, client.Subtract(value))

                ' Call the Multiply service operation.
                value = 9
                Console.WriteLine("  Multiplying by {0}, running total={1}", value, client.Multiply(value))

                ' Call the Divide service operation.
                value = 15
                Console.WriteLine("  Dividing by {0}, running total={1}", value, client.Divide(value))

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
