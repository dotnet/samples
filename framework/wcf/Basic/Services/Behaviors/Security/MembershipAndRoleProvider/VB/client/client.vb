' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel

Namespace Microsoft.Samples.MembershipAndRoleProvider

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            ' Create a client with given client endpoint configuration. Make calls as Alice.
            Dim client As New CalculatorClient()

            ' Set credentials to Alice
            client.ClientCredentials.UserName.UserName = "Alice"
            client.ClientCredentials.UserName.Password = "ecilA-123"

            Try

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

                client.Close()

            Catch e As Exception

                Console.WriteLine("Exception: {0}", e.Message)

                If e.InnerException IsNot Nothing Then
                    Console.WriteLine("Inner Exception: {0}", e.InnerException.Message)
                End If

            End Try

            ' Create a client with given client endpoint configuration. Make calls as Bob.
            client = New CalculatorClient()

            Try

                ' Set credentials to Bob
                client.ClientCredentials.UserName.UserName = "Bob"
                client.ClientCredentials.UserName.Password = "treboR-456"
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

                client.Close()

            Catch e As Exception

                Console.WriteLine("Exception: {0}", e.Message)

                If e.InnerException IsNot Nothing Then
                    Console.WriteLine("Inner Exception: {0}", e.InnerException.Message)
                End If

            End Try

            ' Create a client with given client endpoint configuration. Make calls as Charlie.
            client = New CalculatorClient()

            Try

                ' Set credentials to Charlie
                client.ClientCredentials.UserName.UserName = "Charlie"
                client.ClientCredentials.UserName.Password = "eilrahC-789"
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

                client.Close()

            Catch e As Exception

                Console.WriteLine("Exception: {0}", e.Message)

                If e.InnerException IsNot Nothing Then
                    Console.WriteLine("Inner Exception: {0}", e.InnerException.Message)
                End If

            End Try

            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client.")
            Console.ReadLine()

        End Sub

    End Class

End Namespace

