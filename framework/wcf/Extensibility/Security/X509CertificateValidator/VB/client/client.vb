'  Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System.Security.Cryptography.X509Certificates
Imports System.ServiceModel


Namespace Microsoft.Samples.X509CertificateValidator
    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.
	'Client implementation code.
	Friend Class Client
		Shared Sub Main()
            ' Create a client with Certificate endpoint configuration.
			Dim client As New CalculatorClient("Certificate")

			Try
                client.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My,
                                                                          X509FindType.FindBySubjectName, "alice")

				' Call the Add service operation.
                Dim value1 = 100.0R
                Dim value2 = 15.99R
                Dim result = client.Add(value1, value2)
				Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)

				' Call the Subtract service operation.
				value1 = 145.00R
				value2 = 76.54R
				result = client.Subtract(value1, value2)
				Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result)

				' Call the Multiply service operation.
				value1 = 9.00R
				value2 = 81.25R
				result = client.Multiply(value1, value2)
				Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result)

				' Call the Divide service operation.
				value1 = 22.00R
				value2 = 7.00R
				result = client.Divide(value1, value2)
				Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result)
				client.Close()
			Catch e As TimeoutException
				Console.WriteLine("Call timed out : {0}", e.Message)
				client.Abort()
			Catch e As CommunicationException
				Console.WriteLine("Call failed : {0}", e.Message)
				client.Abort()
			Catch e As Exception
				Console.WriteLine("Call failed : {0}", e.Message)
				client.Abort()
			End Try



			Console.WriteLine()
			Console.WriteLine("Press <ENTER> to terminate client.")
			Console.ReadLine()
		End Sub
	End Class
End Namespace

