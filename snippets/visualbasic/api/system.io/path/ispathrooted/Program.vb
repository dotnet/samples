Imports System.IO

Module Program
    Public Sub Main()
        Dim relative As String = "C:Documents" 
        ShowPathInfo(relative)

        Dim absolute As String = "C:/Documents"
        ShowPathInfo(absolute)
    End Sub

    Private Sub ShowPathInfo(filepath As String)
        Console.WriteLine($"Path: {filepath}")
        Console.WriteLine($"   Rooted: {Path.IsPathRooted(filepath)}")
        Console.WriteLine($"   Fully qualified: {Path.IsPathFullyQualified(filepath)}")
        Console.WriteLine($"   Full path: {Path.GetFullPath(filepath)}")
        Console.WriteLine()
    End Sub
End Module
' The example displays the following output when run on a Windows system:
'    Path: C:Documents
'        Rooted: True
'        Fully qualified: False
'        Full path: c:\Users\user1\Documents\projects\path\ispathrooted\Documents
'
'    Path: C:/Documents
'       Rooted: True
'       Fully qualified: True
'       Full path: C:\Documents