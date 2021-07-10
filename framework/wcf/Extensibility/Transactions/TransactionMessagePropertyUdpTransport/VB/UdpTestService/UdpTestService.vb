'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports Microsoft.ServiceModel.Samples
Imports System.Transactions

Namespace Microsoft.ServiceModel.Samples
	<ServiceContract> _
	Public Interface ICalculatorContract
		<OperationContract> _
		Function Add(ByVal x As Integer, ByVal y As Integer, ByVal clientTransactionId As Guid) As Integer
	End Interface

	<ServiceContract> _
	Public Interface IDatagramContract
		<OperationContract(IsOneWay := True)> _
		Sub Hello()
	End Interface

	<ServiceBehavior(IncludeExceptionDetailInFaults:=True)> _
	Friend Class ConfigurableCalculatorService
		Inherits CalculatorService
	End Class

	Friend Class CalculatorService
		Implements IDatagramContract, ICalculatorContract
        <OperationBehavior(TransactionScopeRequired:=True)> _
        Public Function Add(ByVal x As Integer, ByVal y As Integer,
                      ByVal clientTransactionId As Guid) As Integer Implements ICalculatorContract.Add
            If Transaction.Current.TransactionInformation.DistributedIdentifier = clientTransactionId Then
                Console.WriteLine("The client transaction has flowed to the service")
            Else
                Console.WriteLine("The client transaction has NOT flowed to the service")
            End If

            Console.WriteLine("   adding {0} + {1}", x, y)
            Return (x + y)
        End Function

		Public Sub Hello() Implements IDatagramContract.Hello
			Console.Out.WriteLine("Hello, world!")
		End Sub
	End Class

	Friend Class UdpTestService
		Private Shared Sub ServiceFromCode()
			Console.Out.WriteLine("Testing Udp From Code.")

            Dim datagramBinding As Binding = New CustomBinding(New BinaryMessageEncodingBindingElement(),
                                                               New UdpTransportBindingElement())

			' using the 2-way calculator method requires a session since UDP is not inherently request-response
			Dim calculatorBinding As New SampleProfileUdpBinding(True)
			calculatorBinding.ClientBaseAddress = New Uri("soap.udp://localhost:8003/")

			Dim calculatorAddress As New Uri("soap.udp://localhost:8001/")
			Dim datagramAddress As New Uri("soap.udp://localhost:8002/datagram")

			' we need an http base address so that svcutil can access our metadata
			Dim service As New ServiceHost(GetType(CalculatorService), New Uri("http://localhost:8000/udpsample/"))
			Dim metadataBehavior As New ServiceMetadataBehavior()
            metadataBehavior.HttpGetEnabled = True
            With service
                .Description.Behaviors.Add(metadataBehavior)
                .AddServiceEndpoint(GetType(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex")

                .AddServiceEndpoint(GetType(ICalculatorContract), calculatorBinding, calculatorAddress)
                .AddServiceEndpoint(GetType(IDatagramContract), datagramBinding, datagramAddress)
                .Open()
            End With
            Console.WriteLine("Service is started from code...")
            Console.WriteLine("Press <ENTER> to terminate the service and start service from config...")
            Console.ReadLine()

            service.Close()
        End Sub

		Private Shared Sub ServiceFromConfig()
			Console.Out.WriteLine("Testing Udp From Config.")

			Dim service As New ServiceHost(GetType(ConfigurableCalculatorService))
			service.Open()

			Console.WriteLine("Service is started from config...")
			Console.WriteLine("Press <ENTER> to terminate the service and exit...")
			Console.ReadLine()

			service.Close()
		End Sub

		Shared Sub Main(ByVal args() As String)
			Debug.Listeners.Add(New TextWriterTraceListener(Console.Out))
			ServiceFromCode()
			ServiceFromConfig()
		End Sub
	End Class
End Namespace
