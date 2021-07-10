' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel.Description
Imports System.ServiceModel.Channels
Imports System.IO
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic

Namespace Microsoft.ServiceModel.Samples

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples")> _
    Public Interface IServiceDescriptionCalculator

        <OperationContract()> _
        Function Add(ByVal n1 As Integer, ByVal n2 As Integer) As Integer
        <OperationContract()> _
        Function Subtract(ByVal n1 As Integer, ByVal n2 As Integer) As Integer
        <OperationContract()> _
        Function Multiply(ByVal n1 As Integer, ByVal n2 As Integer) As Integer
        <OperationContract()> _
        Function Divide(ByVal n1 As Integer, ByVal n2 As Integer) As Integer
        <OperationContract()> _
        Function GetServiceDescriptionInfo() As String

    End Interface

    ' Service class which implements the service contract.
    Public Class CalculatorService
        Implements IServiceDescriptionCalculator

        Public Function Add(ByVal n1 As Integer, ByVal n2 As Integer) As Integer Implements IServiceDescriptionCalculator.Add

            Return n1 + n2

        End Function

        Public Function Subtract(ByVal n1 As Integer, ByVal n2 As Integer) As Integer Implements IServiceDescriptionCalculator.Subtract

            Return n1 - n2

        End Function

        Public Function Multiply(ByVal n1 As Integer, ByVal n2 As Integer) As Integer Implements IServiceDescriptionCalculator.Multiply

            Return n1 * n2

        End Function

        Public Function Divide(ByVal n1 As Integer, ByVal n2 As Integer) As Integer Implements IServiceDescriptionCalculator.Divide

            Return n1 / n2

        End Function

        ' Obtain information from the service description as return it as a multi-line string.
        Public Function GetServiceDescriptionInfo() As String Implements IServiceDescriptionCalculator.GetServiceDescriptionInfo

            Dim info As String = ""

            Dim operationContext As OperationContext = System.ServiceModel.OperationContext.Current
            Dim host As ServiceHost = DirectCast(operationContext.Host, ServiceHost)
            Dim desc As ServiceDescription = host.Description

            ' Enumerate the base addresses in the service host.

            info += "Base addresses:" & vbNewLine
            For Each uri As Uri In host.BaseAddresses

                info += "    " & uri.ToString() & vbNewLine

            Next

            ' Enumerate the service endpoints in the service description

            info += "Service endpoints:" & vbNewLine
            For Each endpoint As ServiceEndpoint In desc.Endpoints

                info += "    Address:  " & endpoint.Address.ToString() & vbNewLine
                info += "    Binding:  " & endpoint.Binding.Name & vbNewLine
                info += "    Contract: " & endpoint.Contract.Name & vbNewLine

            Next

            Return info

        End Function

    End Class

End Namespace

