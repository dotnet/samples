Imports System
Imports System.Data
Imports System.Windows.Forms

Public Class Form1
    Inherits Form
    Protected DataSet1 As DataSet  
    
' <Snippet1>
    Private Sub GetConstraints(dataTable As DataTable)
        Console.WriteLine()

        ' Print the table's name.
        Console.WriteLine($"TableName: {dataTable.TableName}")

        ' Iterate through the collection and print 
        ' each name and type value.
        Dim constraint As Constraint
        For Each constraint In  dataTable.Constraints
            Console.WriteLine($"Constraint Name: {constraint.ConstraintName}")
            Console.WriteLine($"Type: {constraint.GetType().ToString()"})

            ' If the constraint is a UniqueConstraint, 
            ' print its properties using a function below.
            If TypeOf constraint Is UniqueConstraint Then
                PrintUniqueConstraintProperties(constraint)
            End If

            ' If the constraint is a ForeignKeyConstraint, 
            ' print its properties using a function below.
            If TypeOf constraint Is ForeignKeyConstraint Then
                PrintForeigKeyConstraintProperties(constraint)
            End If
        Next constraint
    End Sub
    
    Private Sub PrintUniqueConstraintProperties( _
        constraint As Constraint)

        Dim uniqueConstraint = CType(constraint, UniqueConstraint)

        ' Get the Columns as an array.
        Dim columnArray As DataColumn() uniqueConstraint.Columns

        ' Print each column's name.
        For i As Integer = 0 To columnArray.Length - 1
            Console.WriteLine($"Column Name: {columnArray(i).ColumnName}")
        Next
    End Sub
    
    Private Sub PrintForeigKeyConstraintProperties( _
        constraint As Constraint)

        Dim fkConstraint = CType(constraint, ForeignKeyConstraint)
        
        ' Get the Columns as an array.
        Dim columnArray As DataColumn() fkConstraint.Columns
        
        ' Print each column's name.
        For i As Integer = 0 To columnArray.Length - 1
            Console.WriteLine($"Column Name: {columnArray(i).ColumnName}")
        Next
        Console.WriteLine()
        
        ' Get the related columns and print each columns name.
        columnArray = fkConstraint.RelatedColumns
        For i As Integer = 0 To columnArray.Length - 1
            Console.WriteLine($"Related Column Name: {columnArray(i).ColumnName}")
        Next
        Console.WriteLine()
    End Sub
' </Snippet1>
End Class
