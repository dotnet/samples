' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ServiceModel

Namespace Microsoft.Samples.UserName

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.Samples.UserName")> _
    Public Interface ICalculator

        <OperationContract()> _
        Function GetCallerIdentity() As String
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
        Implements ICalculator

        Public Function GetCallerIdentity() As String Implements ICalculator.GetCallerIdentity

            ' By default the username client credential is mapped to a Windows identity
            ' The Windows identity of the caller can be accessed on the ServiceSecurityContext.WindowsIdentity
            Return ServiceSecurityContext.Current.WindowsIdentity.Name

        End Function

        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            Dim result As Double = n1 + n2
            Return result

        End Function

        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            Dim result As Double = n1 - n2
            Return result

        End Function

        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            Dim result As Double = n1 * n2
            Return result

        End Function

        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            Dim result As Double = n1 / n2
            Return result

        End Function

    End Class

End Namespace

