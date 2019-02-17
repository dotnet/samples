Imports System.IO

Module Program
    Public Sub Main()
        Dim path1 As String = "C:/"
        Dim path2 As String = "users/user1/documents"
        Dim path3 As String = "letters"
        ShowPathInformation(path1, path2, path3)
        
        path1 = "D:/"
        path2 =  "/users/user1/documents"
        path3 = "letters"
        ShowPathInformation(path1, path2, path3)

        path1 = "D:/"
        path2 =  "users/user1/documents"
        path3 = "C:/users/user1/documents/data"
        ShowPathInformation(path1, path2, path3)
    End Sub

   Private Sub ShowPathInformation(path1 As String, path2 As String, path3 As String)
        Dim result = Path.Join(path1.AsSpan(), path2.AsSpan(), path3.AsSpan())
        Console.WriteLine($"Concatenating  '{path1}, '{path2}', and `{path3}'")
        Console.WriteLine($"   Path.Join:     '{result}'")
        Console.WriteLine($"   Path.Combine:  '{Path.Combine(path1, path2, path3)}'")
    End Sub
End Module
' The example displays the following output if run on a Windows system:
'   Concatenating  'C:/, 'users/user1/documents', and `letters'
'      Path.Join:     'C:/users/user1/documents\letters'
'      Path.Combine:  'C:/users/user1/documents\letters'
'
'   Concatenating  'D:/, '/users/user1/documents', and `letters'
'      Path.Join:     'D:'users/user1/documents\letters'
'      Path.Combine:  '/users/user1/documents\letters'
'
'   Concatenating  'D:/, 'users/user1/documents', and `C:/users/user1/documents/data'
'      Path.Join:     'D:/users/user1/documents\C:/users/user1/documents/data'
'      Path.Combine:  'C:/users/user1/documents/data'
 