'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Web

Namespace Microsoft.Samples.SimpleAjaxService
    ' Define a service contract.
    <ServiceContract([Namespace]:="SimpleAjaxService")> _
    Public Interface ICalculator
        <WebGet()> _
        Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double
        <WebGet()> _
        Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double
        <WebGet()> _
        Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double
        <WebGet()> _
        Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double
    End Interface

    Public Class CalculatorService
        Implements ICalculator

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
    End Class

End Namespace
