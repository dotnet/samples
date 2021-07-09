'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Discovery
Imports System.ServiceModel.Description

Namespace Microsoft.Samples.Discovery

	Friend Class Client
		Public Shared Sub Main()
            Dim dynamicEndpoint As DynamicEndpoint = New DynamicEndpoint(ContractDescription.GetContract(GetType(ICalculatorService)), New WSHttpBinding())
            Try
                InvokeCalculatorService(dynamicEndpoint)
            Catch ex As EndpointNotFoundException
                Console.WriteLine("Client could not find an endpoint to connect to.")
            End Try

            Console.WriteLine("Press <ENTER> to exit.")
            Console.ReadLine()

		End Sub

		Private Shared Function FindCalculatorServiceAddress() As EndpointAddress
			' Create DiscoveryClient
			Dim discoveryClient As New DiscoveryClient(New UdpDiscoveryEndpoint())

			Console.WriteLine("Finding ICalculatorService endpoints...")
			Console.WriteLine()

			' Find ICalculatorService endpoints            
			Dim findResponse As FindResponse = discoveryClient.Find(New FindCriteria(GetType(ICalculatorService)))

			Console.WriteLine("Found {0} ICalculatorService endpoint(s).", findResponse.Endpoints.Count)
			Console.WriteLine()

			If findResponse.Endpoints.Count > 0 Then
				Return findResponse.Endpoints(0).Address
			Else
				Return Nothing
			End If
		End Function

        Private Shared Sub InvokeCalculatorService(ByVal serviceEndpoint As ServiceEndpoint)
            ' Create a client
            Dim client As New CalculatorServiceClient(serviceEndpoint)

            Console.WriteLine("Invoking CalculatorService")
            Console.WriteLine()

            Dim value1 As Double = 100.0R
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
