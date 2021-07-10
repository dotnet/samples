' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.IdentityModel.Selectors
Imports System.IdentityModel.Tokens
Imports System.ServiceModel

Namespace Microsoft.Samples.TrustedFacade

    Public Class FacadeService
        Implements ICalculator

        Public Function GetCallerIdentity() As String Implements ICalculator.GetCallerIdentity

            Dim client As New CalculatorClient()
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name
            Dim result As String = client.GetCallerIdentity()
            client.Close()
            Return result

        End Function

        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            Dim client As New CalculatorClient()
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name
            Dim result As Double = client.Add(n1, n2)
            client.Close()
            Return result

        End Function

        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            Dim client As New CalculatorClient()
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name
            Dim result As Double = client.Subtract(n1, n2)
            client.Close()
            Return result

        End Function

        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            Dim client As New CalculatorClient()
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name
            Dim result As Double = client.Multiply(n1, n2)
            client.Close()
            Return result

        End Function

        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            Dim client As New CalculatorClient()
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name
            Dim result As Double = client.Divide(n1, n2)
            client.Close()
            Return result

        End Function

        ' Host the service within this EXE console application.
        Public Shared Sub Main()

            ' Create a ServiceHost for the CalculatorService type and provide the base address.
            Using serviceHost As New ServiceHost(GetType(FacadeService))

                ' Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open()

                ' The service can now be accessed.
                Console.WriteLine("The service is ready.")
                Console.WriteLine("Press <ENTER> to terminate service.")
                Console.WriteLine()
                Console.ReadLine()

            End Using

        End Sub

    End Class

    Public Class MyUserNamePasswordValidator
        Inherits UserNamePasswordValidator

        Public Overloads Overrides Sub Validate(ByVal userName As String, ByVal password As String)

            ' check that username matches password
            If userName Is Nothing OrElse userName <> password Then

                Console.WriteLine("Invalid username or password")
                Throw New SecurityTokenValidationException("Invalid username or password")

            End If

        End Sub

    End Class

End Namespace
