Imports System.IO

Module Program
    Public Sub Main()
        Dim basePath As String = Environment.CurrentDirectory
        Dim relativePath As String = "./data/output.xml"
 
        ' Unexpectedly change the current directory.
        Environment.CurrentDirectory = "C:/Users/Public/Documents/"
        
        Dim fullPath As String = Path.GetFullPath(relativePath, basePath)
        Console.WriteLine($"Current directory:\n   {Environment.CurrentDirectory}")
        Console.WriteLine($"Fully qualified path:\n   {fullPath}")
    End Sub
End Module
' The example displays the following output:
'   Current directory:
'      C:\Users\Public\Documents
'   Fully qualified path:
'      C:\Utilities\data\output.xml