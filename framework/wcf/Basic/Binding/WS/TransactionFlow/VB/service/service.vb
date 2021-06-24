' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports System.Transactions
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Globalization

Namespace Microsoft.ServiceModel.Samples

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples")> _
    Public Interface ICalculator

        <OperationContract()> _
        <TransactionFlow(TransactionFlowOption.Mandatory)> _
        Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        <TransactionFlow(TransactionFlowOption.Allowed)> _
        Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        <TransactionFlow(TransactionFlowOption.NotAllowed)> _
        Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double

    End Interface

    ' Service class which implements the service contract.
    <ServiceBehavior(TransactionIsolationLevel:=System.Transactions.IsolationLevel.Serializable)> _
    Public Class CalculatorService
        Implements ICalculator

        <OperationBehavior(TransactionScopeRequired:=True)> _
        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            RecordToLog([String].Format(CultureInfo.CurrentCulture, "Adding {0} to {1}", n1, n2))
            Return n1 + n2

        End Function

        <OperationBehavior(TransactionScopeRequired:=True)> _
        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            RecordToLog([String].Format(CultureInfo.CurrentCulture, "Subtracting {0} from {1}", n2, n1))
            Return n1 - n2

        End Function

        <OperationBehavior(TransactionScopeRequired:=True)> _
        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            RecordToLog([String].Format(CultureInfo.CurrentCulture, "Multiplying {0} by {1}", n1, n2))
            Return n1 * n2

        End Function

        <OperationBehavior(TransactionScopeRequired:=True)> _
        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            RecordToLog([String].Format(CultureInfo.CurrentCulture, "Dividing {0} by {1}", n1, n2))
            Return n1 / n2

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

                    Console.WriteLine("  Writing row to database: {0}", recordText)

                    conn.Close()

                End Using

            Else
                Console.WriteLine("  Noting row: {0}", recordText)
            End If

        End Sub

    End Class

End Namespace
