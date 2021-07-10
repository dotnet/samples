' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel

Namespace Microsoft.ServiceModel.Samples

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            ' Create a client to default (http) endpoint configuration 
            Dim client As New CalculatorClient("default")
            Console.WriteLine("Communicate with http endpoint.")

            ' call operations
            DoCalculations(client)
            'Closing the client gracefully closes the connection and cleans up resources
            client.Close()

            ' Create a client to tcp endpoint configuration
            client = New CalculatorClient("tcp")
            Console.WriteLine("Communicate with tcp endpoint.")
            ' call operations
            DoCalculations(client)
            client.Close()

            ' Create a client to named pipe endpoint configuration
            client = New CalculatorClient("namedpipe")
            Console.WriteLine("Communicate with named pipe endpoint.")
            ' call operations
            DoCalculations(client)
            client.Close()

            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client.")
            Console.ReadLine()

        End Sub

        Private Shared Sub DoCalculations(ByVal client As CalculatorClient)

            ' Call the Add service operation.
            Dim value1 As Double = 100
            Dim value2 As Double = 15.99
            Dim result As Double = client.Add(value1, value2)
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)

            ' Call the Subtract service operation.
            value1 = 145
            value2 = 76.54
            result = client.Subtract(value1, value2)
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result)

            ' Call the Multiply service operation.
            value1 = 9
            value2 = 81.25
            result = client.Multiply(value1, value2)
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result)

            ' Call the Divide service operation.
            value1 = 22
            value2 = 7
            result = client.Divide(value1, value2)
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result)

        End Sub

    End Class

End Namespace
