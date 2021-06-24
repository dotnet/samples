' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Security.Cryptography.X509Certificates
Imports System.IdentityModel.Claims
Imports System.IdentityModel.Policy
Imports System.ServiceModel
Imports System.ServiceModel.Security
Imports System.ServiceModel.Security.Tokens
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher
Imports System.ServiceModel.Channels

Namespace Microsoft.Samples.Identity

    'The service contract is defined in generatedclient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            ' Create a client with given client endpoint configuration
            'DNS identity
            CallService("WSHttpBinding_ICalculator")
            'Certificate identity
            CallService("WSHttpBinding_ICalculator1")
            'RSA identity
            CallService("WSHttpBinding_ICalculator2")
            'UPN identity
            CallService("WSHttpBinding_ICalculator3")

            'Create a custom client identity to authenticate the service
            CallServiceCustomClientIdentity("WSHttpBinding_ICalculator")

            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client.")
            Console.ReadLine()

        End Sub

        Public Shared Sub CallService(ByVal endpointName As String)

            Dim client As New CalculatorClient(endpointName)

            Console.WriteLine("Calling Endpoint: {0}", endpointName)
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
            Console.WriteLine()

            client.Close()

        End Sub

        Public Shared Sub CallServiceCustomClientIdentity(ByVal endpointName As String)

            ' Create a custom binding that sets a custom IdentityVerifier 
            Dim customSecurityBinding As Binding = CreateCustomSecurityBinding()
            ' Call the service with DNS identity, setting a custom EndpointIdentity that checks that the certificate
            ' returned from the service contains an organization name of Contoso in the subject name i.e. O=Contoso 
            Dim serviceAddress As New EndpointAddress(New Uri("http://localhost:8003/servicemodelsamples/service/dnsidentity"), New OrgEndpointIdentity("O=Contoso"))

            Using client As New CalculatorClient(customSecurityBinding, serviceAddress)
                client.ClientCredentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.CurrentUser, StoreName.TrustedPeople, X509FindType.FindBySubjectDistinguishedName, "CN=identity.com, O=Contoso")

                'Setting the certificateValidationMode to PeerOrChainTrust means that if the certificate 
                'is in the user's Trusted People store, then it is trusted without performing a
                'validation of the certificate's issuer chain. This setting is used here for convenience so that the 
                'sample can be run without having to have certificates issued by a certificate authority (CA).
                'This setting is less secure than the default, ChainTrust. The security implications of this 
                'setting should be carefully considered before using PeerOrChainTrust in production code. 
                client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust

                Console.WriteLine("Calling Endpoint: {0}", endpointName)
                ' Call the Add service operation.
                Dim value1 As Double = 100
                Dim value2 As Double = 15.99
                Dim result As Double = client.Add(value1, value2)
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)
                Console.WriteLine()

                ' Call the Subtract service operation.
                value1 = 145
                value2 = 76.54
                result = client.Subtract(value1, value2)
                Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result)

            End Using

        End Sub

        'Create a custom binding using a WsHttpBinding
        Public Shared Function CreateCustomSecurityBinding() As Binding

            Dim binding As New WSHttpBinding(SecurityMode.Message)
            'Clients are anonymous to the service
            binding.Security.Message.ClientCredentialType = MessageCredentialType.None
            'Secure conversation is turned off for simplification. If secure conversation is turned on then 
            'you also need to set the IdentityVerifier on the secureconversation bootstrap binding.
            binding.Security.Message.EstablishSecurityContext = False

            'Get the SecurityBindingElement and cast to a SymmetricSecurityBindingElement to set the IdentityVerifier
            Dim outputBec As BindingElementCollection = binding.CreateBindingElements()
            Dim ssbe As SymmetricSecurityBindingElement = DirectCast(outputBec.Find(Of SecurityBindingElement)(), SymmetricSecurityBindingElement)

            'Set the Custom IdentityVerifier
            ssbe.LocalClientSettings.IdentityVerifier = New CustomIdentityVerifier()
            Return New CustomBinding(outputBec)

        End Function

    End Class

    ' This custom EndpointIdentity stores an organization name as a string value.
    Public Class OrgEndpointIdentity
        Inherits EndpointIdentity

        Private orgClaim As String
        Public Sub New(ByVal orgName As String)

            orgClaim = orgName

        End Sub

        Public Property OrganizationClaim() As String

            Get

                Return orgClaim

            End Get
            Set(ByVal value As String)

                orgClaim = value

            End Set

        End Property

    End Class

    'This custom IdentityVerifier uses the supplied OrgEndpointIdentity to check that 
    'X509 certificate's distinguished name claim contains the organization name e.g. O=Contoso 
    Class CustomIdentityVerifier
        Inherits IdentityVerifier

        Public Overloads Overrides Function CheckAccess(ByVal identity As EndpointIdentity, ByVal authContext As AuthorizationContext) As Boolean

            Dim returnvalue As Boolean = False

            For Each claimset As ClaimSet In authContext.ClaimSets

                For Each claim As Claim In claimset

                    If claim.ClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/x500distinguishedname" Then

                        Dim name As X500DistinguishedName = DirectCast(claim.Resource, X500DistinguishedName)
                        If name.Name.Contains((DirectCast(identity, OrgEndpointIdentity)).OrganizationClaim) Then

                            Console.WriteLine("Claim Type: {0}", claim.ClaimType)
                            Console.WriteLine("Right: {0}", claim.Right)
                            Console.WriteLine("Resource: {0}", claim.Resource)
                            Console.WriteLine()
                            returnvalue = True

                        End If

                    End If

                Next

            Next

            Return returnvalue

        End Function

        Public Overloads Overrides Function TryGetIdentity(ByVal reference As EndpointAddress, ByRef identity As EndpointIdentity) As Boolean

            Return IdentityVerifier.CreateDefault().TryGetIdentity(reference, identity)

        End Function

    End Class

End Namespace

