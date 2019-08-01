Imports System
Imports System.Data
Imports System.Windows.Forms

Public Class Form1
    Inherits Form
    
' <Snippet1>
 Private Sub PrintConstraintNames(myTable As DataTable)
     For Each cs As Constraint In myTable.Constraints
         Console.WriteLine(cs.ConstraintName)
     Next
 End Sub
' </Snippet1>
End Class
