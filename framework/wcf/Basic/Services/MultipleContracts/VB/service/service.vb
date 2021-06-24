' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel

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

    ' Define a second service contract.
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples", SessionMode:=SessionMode.Required)> _
    Public Interface ICalculatorSession

        <OperationContract(IsOneWay:=True)> _
        Sub Clear()
        <OperationContract(IsOneWay:=True)> _
        Sub AddTo(ByVal n As Double)
        <OperationContract(IsOneWay:=True)> _
        Sub SubtractFrom(ByVal n As Double)
        <OperationContract(IsOneWay:=True)> _
        Sub MultiplyBy(ByVal n As Double)
        <OperationContract(IsOneWay:=True)> _
        Sub DivideBy(ByVal n As Double)
        <OperationContract()> _
        Function Result() As Double

    End Interface

    ' Service class which implements the two contracts.
    ' Use an InstanceContextMode of PerSession to maintain the result
    ' An instance of the service will be bound to each session
    <ServiceBehavior(InstanceContextMode:=InstanceContextMode.PerSession)> _
    Public Class CalculatorService
        Implements ICalculator
        Implements ICalculatorSession

        ' Implementation of ICalculator
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

        Private resultNum As Double

        ' Implementation of ICalculatorSession
        Public Sub Clear() Implements ICalculatorSession.Clear

            resultNum = 0

        End Sub

        Public Sub AddTo(ByVal n As Double) Implements ICalculatorSession.AddTo

            resultNum += n

        End Sub

        Public Sub SubtractFrom(ByVal n As Double) Implements ICalculatorSession.SubtractFrom

            resultNum -= n

        End Sub

        Public Sub MultiplyBy(ByVal n As Double) Implements ICalculatorSession.MultiplyBy

            resultNum *= n

        End Sub

        Public Sub DivideBy(ByVal n As Double) Implements ICalculatorSession.DivideBy

            resultNum /= n

        End Sub

        Public Function Result() As Double Implements ICalculatorSession.Result	

            Return resultNum

        End Function

    End Class

End Namespace

