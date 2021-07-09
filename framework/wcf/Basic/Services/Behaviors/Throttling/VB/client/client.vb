' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports System.Threading

Namespace Microsoft.ServiceModel.Samples

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            ' Create a client
            Dim client As New CalculatorClient()

            Console.WriteLine("Press <ENTER> to terminate client once the output is displayed.")
            Console.WriteLine()

            ' BeginAdd
            Dim value1 As Double = 100
            Dim value2 As Double = 15.99
            Dim arAdd As IAsyncResult = client.BeginAdd(value1, value2, AddressOf AddCallback, client)
            Console.WriteLine("Add({0},{1})", value1, value2)

            ' BeginSubstract
            value1 = 145
            value2 = 76.54
            Dim arSubtract As IAsyncResult = client.BeginSubtract(value1, value2, AddressOf SubtractCallback, client)
            Console.WriteLine("Subtract({0},{1})", value1, value2)

            ' BeginMultiply
            value1 = 9
            value2 = 81.25
            Dim arMultiply As IAsyncResult = client.BeginMultiply(value1, value2, AddressOf MultiplyCallback, client)
            Console.WriteLine("Multiply({0},{1})", value1, value2)

            ' BeginDivide
            value1 = 22
            value2 = 7
            Dim arDivide As IAsyncResult = client.BeginDivide(value1, value2, AddressOf DivideCallback, client)
            Console.WriteLine("Divide({0},{1})", value1, value2)

            Console.ReadLine()

            'Closing the client gracefully closes the connection and cleans up resources
            client.Close()

        End Sub

        ' Asynchronous callbacks for displaying results.
        Private Shared Sub AddCallback(ByVal ar As IAsyncResult)

            Dim result As Double = (DirectCast(ar.AsyncState, CalculatorClient)).EndAdd(ar)
            Console.WriteLine("Add Result: {0}", result)

        End Sub

        Private Shared Sub SubtractCallback(ByVal ar As IAsyncResult)

            Dim result As Double = (DirectCast(ar.AsyncState, CalculatorClient)).EndSubtract(ar)
            Console.WriteLine("Subtract Result: {0}", result)

        End Sub

        Private Shared Sub MultiplyCallback(ByVal ar As IAsyncResult)

            Dim result As Double = (DirectCast(ar.AsyncState, CalculatorClient)).EndMultiply(ar)
            Console.WriteLine("Multiply Result: {0}", result)

        End Sub

        Private Shared Sub DivideCallback(ByVal ar As IAsyncResult)

            Dim result As Double = (DirectCast(ar.AsyncState, CalculatorClient)).EndDivide(ar)
            Console.WriteLine("Divide Result: {0}", result)

        End Sub

    End Class

End Namespace

