' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports System.Transactions
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Globalization

Namespace Microsoft.ServiceModel.Samples

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples", SessionMode:=SessionMode.Required)> _
    Public Interface ICalculator

        <OperationContract()> _
        <TransactionFlow(TransactionFlowOption.Mandatory)> _
        Function Add(ByVal n As Double) As Double
        <OperationContract()> _
        <TransactionFlow(TransactionFlowOption.Mandatory)> _
        Function Subtract(ByVal n As Double) As Double
        <OperationContract()> _
        <TransactionFlow(TransactionFlowOption.Mandatory)> _
        Function Multiply(ByVal n As Double) As Double
        <OperationContract()> _
        <TransactionFlow(TransactionFlowOption.Mandatory)> _
        Function Divide(ByVal n As Double) As Double

    End Interface

    ' Service class which implements the service contract.
    <ServiceBehavior(TransactionIsolationLevel:=System.Transactions.IsolationLevel.Serializable, TransactionTimeout:="00:00:30", ReleaseServiceInstanceOnTransactionComplete:=False, TransactionAutoCompleteOnSessionClose:=False)> _
    Public Class CalculatorService
        Implements ICalculator

        Private runningTotal As Double

        Public Sub New()

            Console.WriteLine("Creating new service instance...")

        End Sub

        <OperationBehavior(TransactionScopeRequired:=True, TransactionAutoComplete:=True)> _
        Public Function Add(ByVal n As Double) As Double Implements ICalculator.Add

            RecordToLog([String].Format(CultureInfo.CurrentCulture, "Adding {0} to {1}", n, runningTotal))
            runningTotal = runningTotal + n
            Return runningTotal

        End Function

        <OperationBehavior(TransactionScopeRequired:=True, TransactionAutoComplete:=True)> _
        Public Function Subtract(ByVal n As Double) As Double Implements ICalculator.Subtract

            RecordToLog([String].Format(CultureInfo.CurrentCulture, "Subtracting {0} from {1}", n, runningTotal))
            runningTotal = runningTotal - n
            Return runningTotal

        End Function

        <OperationBehavior(TransactionScopeRequired:=True, TransactionAutoComplete:=False)> _
        Public Function Multiply(ByVal n As Double) As Double Implements ICalculator.Multiply

            RecordToLog([String].Format(CultureInfo.CurrentCulture, "Multiplying {0} by {1}", runningTotal, n))
            runningTotal = runningTotal * n
            Return runningTotal
        End Function

        <OperationBehavior(TransactionScopeRequired:=True, TransactionAutoComplete:=False)> _
        Public Function Divide(ByVal n As Double) As Double Implements ICalculator.Divide

            RecordToLog([String].Format(CultureInfo.CurrentCulture, "Dividing {0} by {1}", runningTotal, n))
            runningTotal = runningTotal / n
            OperationContext.Current.SetTransactionComplete()
            Return runningTotal

        End Function

        Private Shared Sub RecordToLog(ByVal recordText As String)

            ' Record the operations performed
            If ConfigurationManager.AppSettings("usingSql") = "true" Then

                Using conn As New SqlConnection(ConfigurationManager.AppSettings("connectionString"))

                    conn.Open()

                    Dim cmdLog As New SqlCommand("INSERT into Log (Entry) Values (@Entry)", conn)
                    cmdLog.Parameters.AddWithValue("@Entry", recordText)
                    cmdLog.ExecuteNonQuery()
                    cmdLog.Dispose()

                    Dim cmdCount As New SqlCommand("SELECT COUNT(*) FROM Log", conn)
                    Dim rowCount As String = cmdCount.ExecuteScalar().ToString()
                    cmdCount.Dispose()

                    Console.WriteLine("  Writing row {0} to database: {1}", rowCount, recordText)

                    conn.Close()

                End Using

            Else
                Console.WriteLine("  Noting row: {0}", recordText)
            End If

        End Sub

    End Class

End Namespace
