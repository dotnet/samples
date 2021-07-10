' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.IdentityModel.Claims
Imports System.IdentityModel.Selectors
Imports System.IdentityModel.Tokens
Imports System.ServiceModel

Namespace Microsoft.Samples.TrustedFacade

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.Samples.TrustedFacade")> _
    Public Interface ICalculator

        <OperationContract()> _
        Function GetCallerIdentity() As String
        <OperationContract()> _
        Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double

    End Interface

    ' Service class which implements the service contract.
    Public Class BackendService
        Implements ICalculator

        Public Function GetCallerIdentity() As String Implements ICalculator.GetCallerIdentity

            ' Facade service is authenticated using Windows authentication. Its identity is accessible
            ' on ServiceSecurityContext.Current.WindowsIdentity
            Dim facadeServiceIdentityName As String = ServiceSecurityContext.Current.WindowsIdentity.Name

            ' The client name is transmitted using Username authentication on the message level without the password
            ' using a supporting encrypted UserNameToken
            ' Claims extracted from this supporting token are available in 
            ' ServiceSecurityContext.Current.AuthorizationContext.ClaimSets collection.
            Dim clientName As String = Nothing
            For Each claimSet As ClaimSet In ServiceSecurityContext.Current.AuthorizationContext.ClaimSets

                For Each claim As Claim In claimSet

                    If claim.ClaimType = ClaimTypes.Name AndAlso claim.Right = Rights.Identity Then

                        clientName = DirectCast(claim.Resource, String)
                        Exit For

                    End If

                Next

            Next
            If clientName Is Nothing Then

                ' In case there was no UserNameToken attached to the request
                ' In the real world implementation the service will very likely reject such request.
                Return "Anonymous caller via " + facadeServiceIdentityName

            End If

            Return clientName + " via " + facadeServiceIdentityName

        End Function

        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            Return n1 + n2

        End Function

        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            Return n1 - n2

        End Function

        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            Return n1 * n2

        End Function

        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            Return n1 / n2

        End Function

        Public Shared Sub Main()

            ' Create a ServiceHost for the CalculatorService type and provide the base address.
            Using serviceHost As New ServiceHost(GetType(BackendService))

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

            ' ignore the password because it is empty, we trust the facade service to authenticate the client
            ' we just accept the username information here so that application gets access to it
            If userName Is Nothing Then

                Console.WriteLine("Invalid username")
                Throw New SecurityTokenValidationException("Invalid username")

            End If

        End Sub

    End Class

End Namespace

