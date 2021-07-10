'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Runtime.Serialization

Namespace Microsoft.Samples.XmlAjaxService
    ' Define a service contract.
    <ServiceContract([Namespace]:="XmlAjaxService")> _
    Public Interface ICalculator
        <WebInvoke(ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Wrapped)> _
        Function DoMathJson(ByVal n1 As Double, ByVal n2 As Double) As MathResult

        <WebInvoke(ResponseFormat:=WebMessageFormat.Xml, BodyStyle:=WebMessageBodyStyle.Wrapped)> _
        Function DoMathXml(ByVal n1 As Double, ByVal n2 As Double) As MathResult

    End Interface

    Public Class CalculatorService
        Implements ICalculator

        Public Function DoMathJson(ByVal n1 As Double, ByVal n2 As Double) As MathResult Implements ICalculator.DoMathJson
            Return DoMath(n1, n2)
        End Function

        Public Function DoMathXml(ByVal n1 As Double, ByVal n2 As Double) As MathResult Implements ICalculator.DoMathXml
            Return DoMath(n1, n2)
        End Function

        Public Function DoMath(ByVal n1 As Double, ByVal n2 As Double) As MathResult
            Dim mr As New MathResult()
            mr.sum = n1 + n2
            mr.difference = n1 - n2
            mr.product = n1 * n2
            mr.quotient = n1 / n2
            Return mr
        End Function
    End Class

    <DataContract()> _
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
