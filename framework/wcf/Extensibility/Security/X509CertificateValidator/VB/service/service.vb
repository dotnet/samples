'  Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System.IdentityModel.Selectors
Imports System.IdentityModel.Tokens
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Principal
Imports System.ServiceModel

Namespace Microsoft.Samples.X509CertificateValidator
	' Define a service contract.
    <ServiceContract(Namespace:="http://Microsoft.Samples.X509CertificateValidator")>
 Public Interface ICalculator
        <OperationContract()>
        Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()>
        Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()>
        Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()>
        Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double
    End Interface

	' Service class that implements the service contract.
    ' Added code to write output to the console window.
    <ServiceBehavior(IncludeExceptionDetailInFaults:=True)>
 Public Class CalculatorService
        Implements ICalculator
        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add
            Dim result = n1 + n2
            Console.WriteLine("Received Add({0},{1})", n1, n2)
            Console.WriteLine("Return: {0}", result)
            Return result
        End Function


        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract
            Dim result = n1 - n2
            Console.WriteLine("Received Subtract({0},{1})", n1, n2)
            Console.WriteLine("Return: {0}", result)
            Return result
        End Function


        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply
            Dim result = n1 * n2
            Console.WriteLine("Received Multiply({0},{1})", n1, n2)
            Console.WriteLine("Return: {0}", result)
            Return result
        End Function


        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide
            Dim result = n1 / n2
            Console.WriteLine("Received Divide({0},{1})", n1, n2)
            Console.WriteLine("Return: {0}", result)
            Return result
        End Function
        ' Host the service within this EXE console application.
        Public Shared Sub Main()
            ' Create a ServiceHost for the CalculatorService type.
            Using serviceHost As New ServiceHost(GetType(CalculatorService))
                ' Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom
                serviceHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = New CustomX509CertificateValidator()

                serviceHost.Open()

                ' The service can now be accessed.
                Console.WriteLine("The service is ready.")
                Console.WriteLine("The service is running in the following account: {0}", WindowsIdentity.GetCurrent().Name)
                Console.WriteLine("Press <ENTER> to terminate service.")
                Console.WriteLine()
                Console.ReadLine()

            End Using
        End Sub
    End Class

	Public Class CustomX509CertificateValidator
		Inherits IdentityModel.Selectors.X509CertificateValidator
		' This Validation function accepts any X.509 Certificate that is self-issued. As anyone can construct such
		' a certificate this custom validator is less secure than the default behavior provided by the
		' ChainTrust X509CertificateValidationMode. The security implications of this should be carefully 
		' considered before using this validation logic in production code. 
		Public Overrides Sub Validate(ByVal certificate As X509Certificate2)
			' Check that we have been passed a certificate
			If certificate Is Nothing Then
				Throw New ArgumentNullException("certificate")
			End If

            ' Only accept self-issued certificates.
			If certificate.Subject <> certificate.Issuer Then
                'Throw New SecurityTokenException("Certificate is not self-issued")
			End If
		End Sub
	End Class
End Namespace

