' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Configuration
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.ServiceModel
Imports System.ServiceModel.Security
Imports System.ServiceModel.Description
Imports System.ServiceModel.Channels
Imports System.IdentityModel.Selectors
Imports System.ServiceModel.Dispatcher

Namespace Microsoft.Samples.Identity

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.Samples.Identity")> _
    Public Interface ICalculator
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
    ' Added code to write output to the console window
    Public Class CalculatorService
        Implements ICalculator

        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            Dim result As Double = n1 + n2
            Console.WriteLine("Received Add({0},{1})", n1, n2)
            Console.WriteLine("Return: {0}", result)
            Return result

        End Function

        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            Dim result As Double = n1 - n2
            Console.WriteLine("Received Subtract({0},{1})", n1, n2)
            Console.WriteLine("Return: {0}", result)
            Return result

        End Function

        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            Dim result As Double = n1 * n2
            Console.WriteLine("Received Multiply({0},{1})", n1, n2)
            Console.WriteLine("Return: {0}", result)
            Return result

        End Function

        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            Dim result As Double = n1 / n2
            Console.WriteLine("Received Divide({0},{1})", n1, n2)
            Console.WriteLine("Return: {0}", result)
            Return result

        End Function

        ' Host the service within this EXE console application.
        Public Shared Sub Main()

            ' Create a ServiceHost for the CalculatorService type. Base Address is supplied in app.config
            Using serviceHost As New ServiceHost(GetType(CalculatorService))

                ' The base address is read from the app.config
                Dim dnsrelativeAddress As New Uri(serviceHost.BaseAddresses(0), "dnsidentity")
                Dim certificaterelativeAddress As New Uri(serviceHost.BaseAddresses(0), "certificateidentity")
                Dim rsarelativeAddress As New Uri(serviceHost.BaseAddresses(0), "rsaidentity")

                ' Set the service's X509Certificate to protect the messages
                serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectDistinguishedName, "CN=identity.com, O=Contoso")

                'cache a reference to the server's certificate.
                Dim servercert As X509Certificate2 = serviceHost.Credentials.ServiceCertificate.Certificate

                'Create endpoints for the service using a WSHttpBinding set for anonymous clients
                Dim wsAnonbinding As New WSHttpBinding(SecurityMode.Message)
                'Clients are anonymous to the service
                wsAnonbinding.Security.Message.ClientCredentialType = MessageCredentialType.None
                'Secure conversation (session) is turned off
                wsAnonbinding.Security.Message.EstablishSecurityContext = False

                'Create a service endpoint and change its identity to the DNS for an X509 Certificate
                Dim ep As ServiceEndpoint = serviceHost.AddServiceEndpoint(GetType(ICalculator), wsAnonbinding, [String].Empty)
                Dim epa As New EndpointAddress(dnsrelativeAddress, EndpointIdentity.CreateDnsIdentity("identity.com"))
                ep.Address = epa

                'Create a service endpoint and change its identity to the X509 certificate returned as base64 encoded value
                Dim ep2 As ServiceEndpoint = serviceHost.AddServiceEndpoint(GetType(ICalculator), wsAnonbinding, [String].Empty)
                Dim epa2 As New EndpointAddress(certificaterelativeAddress, EndpointIdentity.CreateX509CertificateIdentity(servercert))
                ep2.Address = epa2

                'Create a service endpoint and change its identity to the X509 certificate's RSA key value
                Dim ep3 As ServiceEndpoint = serviceHost.AddServiceEndpoint(GetType(ICalculator), wsAnonbinding, [String].Empty)
                Dim epa3 As New EndpointAddress(rsarelativeAddress, EndpointIdentity.CreateRsaIdentity(servercert))
                ep3.Address = epa3

                ' Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open()

                ' List the address and the identity for each ServiceEndpoint
                For Each channelDispatcher As ChannelDispatcher In serviceHost.ChannelDispatchers

                    For Each endpointDispatcher As EndpointDispatcher In channelDispatcher.Endpoints

                        Console.WriteLine("Endpoint Address: {0}", endpointDispatcher.EndpointAddress)
                        Console.WriteLine("Endpoint Identity: {0}", endpointDispatcher.EndpointAddress.Identity)
                        Console.WriteLine()

                    Next

                Next

                'foreach (ServiceEndpoint endpoint in serviceHost.Description.Endpoints)
                '{
                '    object resource = endpoint.Address.Identity.IdentityClaim.Resource;
                '    string identityValue;
                '    if (resource.GetType() == typeof(System.Byte[]))
                '    {
                '        identityValue = System.Convert.ToBase64String((System.Byte[])(resource));
                '    }
                '    else
                '        identityValue = resource.ToString();
                '    Console.WriteLine("Service listening Address: {0}", endpoint.Address);
                '    Console.WriteLine("Service listening Identity: {0}", identityValue);
                '    Console.WriteLine();
                '}

                ' The service can now be accessed.
                Console.WriteLine()
                Console.WriteLine("The service is ready.")
                Console.WriteLine("Press <ENTER> to terminate service.")
                Console.WriteLine()
                Console.ReadLine()

                ' Close the ServiceHostBase to shutdown the service.
                serviceHost.Close()

            End Using

        End Sub

        ' Method for retreving a named certificate from a particular store.
        Private Shared Function GetServerCertificate(ByVal name As String) As X509Certificate2

            Dim store As New X509Store(StoreLocation.LocalMachine)
            store.Open(OpenFlags.[ReadOnly])
            Dim certs As X509Certificate2Collection = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, name, False)
            store.Close()
            If certs.Count = 0 Then
                Throw New Exception("You have not installed the certificates. Run setup.bat for this project")
            End If
            If certs.Count > 1 Then
                Throw New Exception("Duplicate certificates found in the store")
            End If
            Return certs(0)

        End Function

    End Class

End Namespace

