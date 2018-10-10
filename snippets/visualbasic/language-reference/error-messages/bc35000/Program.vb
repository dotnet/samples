Imports Microsoft.VisualBasic

Module Program
    Sub Main(args() As String)
       Dim p As New Person()
       Dim obj = CObj(p)
    End Sub

    Public Sub Test()
        Dim myObject As Object = CreateObject()
        myObject.Add("Adam")

        'Dim filename As String = ""
        'myObject.Attachment.Add(filename) ' <--  error occurs here
    End Sub

    Private Function CreateObject() As Object
        Return New Collection
        'Throw New NotImplementedException()
    End Function
End Module

Public Class Person
   Public Name As String
End Class