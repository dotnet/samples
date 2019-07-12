Imports System
Imports System.Data
Imports System.Windows.Forms


Public Class Form1: Inherits Form


' <Snippet1>
Private Sub PrintColumnNames(dataSet As DataSet)
    ' For each DataTable, print the ColumnName.
    For Each table As DataTable In dataSet.Tables
        For Each column As DataColumn In table.Columns
            Console.WriteLine(column.ColumnName)
        Next
    Next
End Sub

Private Sub AddColumn(table As DataTable)
    Dim column As New DataColumn()

    With column
        .ColumnName = "SupplierID"
        .DataType = System.Type.GetType("System.String")
        .Unique = True
        .AutoIncrement = False
        .Caption = "SupplierID"
        .ReadOnly = False
    End With

    ' Add the column to the table's columns collection.
    table.Columns.Add(column)
End Sub
 ' </Snippet1>

End Class
