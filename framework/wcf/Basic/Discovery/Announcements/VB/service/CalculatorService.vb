'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.ServiceModel

Namespace Microsoft.Samples.Discovery

	' Define a service contract.
    <ServiceContract(Namespace:="http://Microsoft.Samples.Discovery")> _
    Public Interface ICalculatorService

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
	Public Class CalculatorService
		Implements ICalculatorService
		Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculatorService.Add
			Dim result As Double = n1 + n2
			Console.WriteLine("Received Add({0},{1})", n1, n2)
			Console.WriteLine("Return: {0}", result)
			Return result
		End Function

		Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculatorService.Subtract
			Dim result As Double = n1 - n2
			Console.WriteLine("Received Subtract({0},{1})", n1, n2)
			Console.WriteLine("Return: {0}", result)
			Return result
		End Function

		Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculatorService.Multiply
			Dim result As Double = n1 * n2
			Console.WriteLine("Received Multiply({0},{1})", n1, n2)
			Console.WriteLine("Return: {0}", result)
			Return result
		End Function

		Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculatorService.Divide
			Dim result As Double = n1 / n2
			Console.WriteLine("Received Divide({0},{1})", n1, n2)
			Console.WriteLine("Return: {0}", result)
			Return result
		End Function
	End Class
End Namespace
