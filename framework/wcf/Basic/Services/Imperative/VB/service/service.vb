' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel.Channels
Imports System.ServiceModel

Namespace Microsoft.ServiceModel.Samples

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples")> _
    Public Interface ICalculator

        <OperationContract(IsOneWay:=False)> _
        Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract(IsOneWay:=False)> _
        Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract(IsOneWay:=False)> _
        Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract(IsOneWay:=False)> _
        Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double

    End Interface

    ' Service class which implements the service contract.
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

            Dim binding As New WSHttpBinding()
            binding.Name = "binding1"
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard
            binding.Security.Mode = SecurityMode.Message
            binding.ReliableSession.Enabled = False
            binding.TransactionFlow = False

            Dim baseAddress As New Uri("http://localhost:8000/servicemodelsamples/service")

            ' Create a ServiceHost for the CalculatorService type and provide the base address.
            Using serviceHost As New ServiceHost(GetType(CalculatorService), baseAddress)

                serviceHost.AddServiceEndpoint(GetType(ICalculator), binding, baseAddress)
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

End Namespace

