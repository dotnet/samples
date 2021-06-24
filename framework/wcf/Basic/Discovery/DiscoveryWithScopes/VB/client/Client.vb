'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Discovery

Namespace Microsoft.Samples.Discovery

	Friend Class Client
		Public Shared Sub Main()
			Dim endpointAddress As EndpointAddress = FindCalculatorServiceAddress()

			If endpointAddress IsNot Nothing Then
				InvokeCalculatorService(endpointAddress)
			End If

			Console.WriteLine("Press <ENTER> to exit.")
			Console.ReadLine()
		End Sub

		Private Shared Function FindCalculatorServiceAddress() As EndpointAddress
			' Create DiscoveryClient
			Dim discoveryClient As New DiscoveryClient(New UdpDiscoveryEndpoint())

			' Find ICalculatorService endpoints in the specified scope            
			Dim scope As New Uri("ldap:///ou=engineering,o=exampleorg,c=us")
			Dim findCriteria As New FindCriteria(GetType(ICalculatorService))
			findCriteria.Scopes.Add(scope)
			findCriteria.MaxResults = 1

			Console.WriteLine("Finding ICalculatorService endpoints within {0} scope...", scope)
			Console.WriteLine()

			Dim findResponse As FindResponse = discoveryClient.Find(findCriteria)

			Console.WriteLine("Found {0} ICalculatorService endpoint(s).", findResponse.Endpoints.Count)
			Console.WriteLine()

			If findResponse.Endpoints.Count > 0 Then
				Return findResponse.Endpoints(0).Address
			Else
				Return Nothing
			End If
		End Function

		Private Shared Sub InvokeCalculatorService(ByVal endpointAddress As EndpointAddress)
			' Create a client
			Dim client As New CalculatorServiceClient()

			' Connect to the discovered service endpoint
			client.Endpoint.Address = endpointAddress

			Console.WriteLine("Invoking CalculatorService at {0}", endpointAddress)

			Dim value1 As Double = 100.00R
			Dim value2 As Double = 15.99R

			' Call the Add service operation.
			Dim result As Double = client.Add(value1, value2)
			Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)

			' Call the Subtract service operation.
			result = client.Subtract(value1, value2)
			Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result)

			' Call the Multiply service operation.
			result = client.Multiply(value1, value2)
			Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result)

			' Call the Divide service operation.
			result = client.Divide(value1, value2)
			Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result)
			Console.WriteLine()

			'Closing the client gracefully closes the connection and cleans up resources
			client.Close()
		End Sub
	End Class
End Namespace

