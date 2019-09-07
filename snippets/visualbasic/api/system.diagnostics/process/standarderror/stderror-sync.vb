Imports System.Diagnostics

Public Module Example
    Public Sub Main()
        Dim p As New Process()
        p.StartInfo.UseShellExecute = False  
        p.StartInfo.RedirectStandardError = True  
        p.StartInfo.FileName = "Write500Lines.exe"  
        p.Start() 

        ' To avoid deadlocks, always read the output stream first and then wait.  
        Dim output As String = p.StandardError.ReadToEnd()  
        p.WaitForExit()

        Console.WriteLine($"{vbCrLf}Error stream: {output}")
    End Sub
End Module
' The end of the output produced by the example includes the following:
'      Error stream:
'      Successfully wrote 500 lines.
