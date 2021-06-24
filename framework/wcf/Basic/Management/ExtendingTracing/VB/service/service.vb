' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Data
Imports System.Diagnostics
Imports System.ServiceModel

Namespace Microsoft.Samples.ServiceModel

    ' Define a service contract.
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

    ' Service class which implements the service contract.
    Public Class CalculatorService
        Implements ICalculator

        Private ts As TraceSource

        Public Sub New()

            ts = New TraceSource("ServerCalculatorTraceSource")

        End Sub

        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            If Trace.CorrelationManager.ActivityId = Guid.Empty Then

                Dim newGuid As Guid = Guid.NewGuid()
                Trace.CorrelationManager.ActivityId = newGuid
            End If

            ts.TraceEvent(TraceEventType.Start, 0, "Add Activity")
            ts.TraceEvent(TraceEventType.Information, 0, "Service receives Add request message.")

            Dim result As Double = n1 + n2

            ts.TraceEvent(TraceEventType.Information, 0, "Service sends Add response message.")
            ts.TraceEvent(TraceEventType.[Stop], 0, "Add Activity")

            Return result

        End Function

        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            If Trace.CorrelationManager.ActivityId = Guid.Empty Then

                Dim newGuid As Guid = Guid.NewGuid()
                Trace.CorrelationManager.ActivityId = newGuid

            End If

            ts.TraceEvent(TraceEventType.Start, 0, "Subtract Activity")
            ts.TraceEvent(TraceEventType.Information, 0, "Service receives Subtract request message.")

            Dim result As Double = n1 - n2

            ts.TraceEvent(TraceEventType.Information, 0, "Service sends Subtract response message.")
            ts.TraceEvent(TraceEventType.[Stop], 0, "Subtract Activity")

            Return result

        End Function

        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            If Trace.CorrelationManager.ActivityId = Guid.Empty Then

                Dim newGuid As Guid = Guid.NewGuid()
                Trace.CorrelationManager.ActivityId = newGuid

            End If

            ts.TraceEvent(TraceEventType.Start, 0, "Multiply Activity")
            ts.TraceEvent(TraceEventType.Information, 0, "Service receives Multiply request message.")

            Dim result As Double = n1 * n2

            ts.TraceEvent(TraceEventType.Information, 0, "Service sends Multiply response message.")
            ts.TraceEvent(TraceEventType.[Stop], 0, "Multiply Activity")

            Return result

        End Function

        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            If Trace.CorrelationManager.ActivityId = Guid.Empty Then

                Dim newGuid As Guid = Guid.NewGuid()
                Trace.CorrelationManager.ActivityId = newGuid

            End If

            ts.TraceEvent(TraceEventType.Start, 0, "Divide Activity")
            ts.TraceEvent(TraceEventType.Information, 0, "Service receives Divide request message.")

            Dim result As Double = n1 / n2

            ts.TraceEvent(TraceEventType.Information, 0, "Service sends Divide response message.")
            ts.TraceEvent(TraceEventType.[Stop], 0, "Divide Activity")

            Return result

        End Function

    End Class

End Namespace
