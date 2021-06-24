' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Management
Imports System.Management.Instrumentation
Imports System.ServiceModel

Namespace Microsoft.Samples.ServiceModel

    ' Define an instrumentation class.
    <InstrumentationClass(InstrumentationType.Instance)> _
    Public Class WMIObject

        Public WMIInfo As String = "User Defined WMI Information."

        Public Sub ChangeInfo(ByVal newInfo As String)

            WMIInfo = newInfo

        End Sub

    End Class

    ' Define service contract.
    <ServiceContract([Namespace]:="http://Microsoft.Samples.ServiceModel")> _
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

    ' Let the system know that the InstallUtil.exe tool will be run
    ' against this assembly in order to register the .dll's schema to WMI.
    <System.ComponentModel.RunInstaller(True)> _
    Public Class MyInstaller
        Inherits DefaultManagementProjectInstaller

        Public Sub New()

            Dim mgmtInstaller As New ManagementInstaller()
            Installers.Add(mgmtInstaller)

        End Sub

    End Class

    ' Service class which implements the service contract.
    Public Class CalculatorService
        Implements ICalculator

        Private wmiObj As WMIObject

        Public Sub New()

            wmiObj = New WMIObject()

            'publish the object to WMI in order to be viewed
            Instrumentation.Publish(wmiObj)

        End Sub

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
