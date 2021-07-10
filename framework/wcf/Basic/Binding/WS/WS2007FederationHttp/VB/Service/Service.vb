' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System
Imports System.Security.Permissions
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports System.ServiceModel.Security

Namespace Microsoft.Samples.WS2007FederationHttpBinding
    <ServiceBehavior(IncludeExceptionDetailInFaults:=True)> _
    Class CalculatorService
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
    End Class

    Class Service
        Shared Sub Main(ByVal args As String())
            ' Create service host
            Dim sh As New ServiceHost(GetType(CalculatorService))

            ' Setting the certificateValidationMode to PeerOrChainTrust means that if the certificate 
            ' is in the user's Trusted People store, then it will be trusted without performing a
            ' validation of the certificate's issuer chain. This setting is used here for convenience so that the 
            ' sample can be run without having to have certificates issued by a certificate authority (CA).
            ' This setting is less secure than the default, ChainTrust. The security implications of this 
            ' setting should be carefully considered before using PeerOrChainTrust in production code. 
            sh.Credentials.IssuedTokenAuthentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust
            sh.Open()

            Try
                For Each cd As ChannelDispatcher In sh.ChannelDispatchers
                    For Each ed As EndpointDispatcher In cd.Endpoints
                        Console.WriteLine("Service listening at {0}", ed.EndpointAddress.Uri)
                    Next
                Next

                Console.WriteLine("Press enter to close the service")
                Console.ReadLine()
            Finally
                sh.Close()
            End Try
        End Sub
    End Class
End Namespace
