'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Runtime.Serialization

Namespace Microsoft.Samples.ComplexTypeAjaxService
    ' Define a service contract.
    <ServiceContract([Namespace]:="ComplexTypeAjaxService")> _
    Public Interface ICalculator
        <OperationContract()> _
        Function DoMath(ByVal n1 As Double, ByVal n2 As Double) As MathResult
    End Interface

    Public Class CalculatorService
        Implements ICalculator
        Public Function DoMath(ByVal n1 As Double, ByVal n2 As Double) As MathResult Implements ICalculator.DoMath
            Dim mr As New MathResult()
            mr.sum = n1 + n2
            mr.difference = n1 - n2
            mr.product = n1 * n2
            mr.quotient = n1 / n2
            Return mr
        End Function
    End Class

    <DataContract([Namespace]:="http://www.example.com/AjaxCalculator")> _
    Public Class MathResult
        <DataMember()> _
        Public sum As Double

        <DataMember()> _
        Public difference As Double

        <DataMember()> _
        Public product As Double

        <DataMember()> _
        Public quotient As Double
    End Class

End Namespace
