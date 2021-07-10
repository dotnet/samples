'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports System
Imports System.ServiceModel

Namespace Microsoft.Samples.Discovery

	Friend Class Client
        Public Shared Sub Main()
            Try
                InvokeCalculatorService()
            Catch ex As EndpointNotFoundException
                Console.WriteLine("Client was unable to find an endpoint to connect to")
            End Try
            Console.WriteLine("Press <ENTER> to exit.")
            Console.ReadLine()
        End Sub

        Private Shared Sub InvokeCalculatorService()
            ' Create a client
            Dim client As New CalculatorServiceClient("calculatorEndpoint")

            Console.WriteLine("Invoking CalculatorService")

            Dim value1 As Double = 100.0R
            Dim value2 As Double = 15.99R

            ' Call the Add service operation.
            Dim result As Double = client.Add(value1, value2)
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)

            ' Call the Subtract service operation.
            result = client.Subtract(value1, value2)
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result)

            ' Call the Multiply service operation.
            result = client.Multiply(value1, value2)
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result)

            ' Call the Divide service operation.
            result = client.Divide(value1, value2)
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result)
            Console.WriteLine()

            'Closing the client gracefully closes the connection and cleans up resources
            client.Close()
        End Sub
    End Class
End Namespace

