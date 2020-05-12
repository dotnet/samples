Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting

Public Module MefCalculatorInterfaces

    Public Interface ICalculator
        Function Calculate(input As String) As String
    End Interface

    Public Interface IOperation
        Function Operate(left As Integer, right As Integer) As Integer
    End Interface

    Public Interface IOperationData
        ReadOnly Property Symbol As Char
    End Interface


    <Export(GetType(IOperation))>
    <ExportMetadata("Symbol", "+"c)>
    Public Class Add
        Implements IOperation

        Public Function Operate(left As Integer, right As Integer) As Integer Implements IOperation.Operate
            Return left + right
        End Function
    End Class

    <Export(GetType(IOperation))>
    <ExportMetadata("Symbol", "-"c)>
    Public Class Subtract
        Implements IOperation

        Public Function Operate(left As Integer, right As Integer) As Integer Implements IOperation.Operate
            Return left - right
        End Function
    End Class

End Module
