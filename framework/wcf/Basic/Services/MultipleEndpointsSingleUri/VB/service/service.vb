' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports Microsoft.VisualBasic

Namespace Microsoft.ServiceModel.Samples

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

    ' Define a second contract
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples")> _
    Public Interface IEcho

        <OperationContract()> _
        Function Echo(ByVal s As String) As String

    End Interface

    ' Service class which implements the service contracts.
    Public Class CalculatorService
        Implements ICalculator
        Implements IEcho

        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            Return n1 + n2

        End Function

        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            Return n1 - n2

        End Function

        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            Return n1 * n2

        End Function

        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            Return n1 / n2

        End Function

        Public Function Echo(ByVal s As String) As String Implements IEcho.Echo

            Dim addressIncomingMessageWasSentTo As String = OperationContext.Current.IncomingMessageHeaders.[To].ToString()
            Return s & vbNewLine & "(Message was sent To " & addressIncomingMessageWasSentTo & ")"

        End Function

    End Class

End Namespace
