Imports System
Imports System.Data
Imports System.Windows.Forms

Public Class Form1
    Inherits Form
    Protected DataSet1 As DataSet
    
' <Snippet1>
 Private Sub AddConstraint(table As DataTable)
     ' Assuming a column named "UniqueColumn" exists, and 
     ' its Unique property is true.
     Dim uniqueConstraint As New UniqueConstraint(table.Columns("UniqueColumn"))
     table.Constraints.Add(uniqueConstraint)
 End Sub
' </Snippet1>
End Class
