' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Text
Imports System.ServiceModel

Namespace Microsoft.Samples.UserName

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            Console.WriteLine("Username authentication required.")
            Console.WriteLine("Provide a valid machine or domain account. [domain\user]")
            Console.WriteLine("   Enter username:")
            Dim username As String = Console.ReadLine()
            Console.WriteLine("   Enter password:")
            Dim password As New StringBuilder()

            Dim info As ConsoleKeyInfo = Console.ReadKey(True)
            While info.Key <> ConsoleKey.Enter

                If info.Key <> ConsoleKey.Backspace Then

                    password.Append(info.KeyChar)
                    info = Console.ReadKey(True)

                ElseIf info.Key = ConsoleKey.Backspace Then

                    If password.Length <> 0 Then

                        password.Remove(password.Length - 1, 1)

                    End If
                    info = Console.ReadKey(True)

                End If

            End While

            For i As Integer = 0 To password.Length - 1
                Console.Write("*")
            Next

            Console.WriteLine()

            ' Create a client
            Dim client As New CalculatorClient()

            ' Configure client with valid machine or domain account (username,password)
            client.ClientCredentials.UserName.UserName = username
            client.ClientCredentials.UserName.Password = password.ToString()

            ' Call GetCallerIdentity service operation
            Console.WriteLine(client.GetCallerIdentity())

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

            'Closing the client gracefully closes the connection and cleans up resources
            client.Close()
            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client.")
            Console.ReadLine()

        End Sub

    End Class

End Namespace

