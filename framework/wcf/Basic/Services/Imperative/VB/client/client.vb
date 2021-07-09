' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Channels

' Define a service contract.
<ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples")> _
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

Namespace Microsoft.ServiceModel.Samples

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            Dim binding As New WSHttpBinding()
            binding.Name = "binding1"
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard
            binding.Security.Mode = SecurityMode.Message
            binding.ReliableSession.Enabled = False
            binding.TransactionFlow = False

            Dim url As String = "http://localhost:8000/servicemodelsamples/service"
            Dim address As New EndpointAddress(url)
            Dim channelFactory As New ChannelFactory(Of ICalculator)(binding, address)
            Dim channel As ICalculator = channelFactory.CreateChannel()

            ' Call the Add service operation.
            Dim value1 As Double = 100
            Dim value2 As Double = 15.99
            Dim result As Double = channel.Add(value1, value2)
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)

            ' Call the Subtract service operation.
            value1 = 145
            value2 = 76.54
            result = channel.Subtract(value1, value2)
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result)

            ' Call the Multiply service operation.
            value1 = 9
            value2 = 81.25
            result = channel.Multiply(value1, value2)
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result)

            ' Call the Divide service operation.
            value1 = 22
            value2 = 7
            result = channel.Divide(value1, value2)

            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result)
            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client.")
            DirectCast(channel, IChannel).Close()
            Console.ReadLine()

        End Sub

    End Class

End Namespace
