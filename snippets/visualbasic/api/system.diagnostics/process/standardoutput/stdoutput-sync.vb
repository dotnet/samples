Imports System.Diagnostics'

Public Module Example
   Public Sub Main()
      Dim p As New Process()
      p.StartInfo.UseShellExecute = False  
      p.StartInfo.RedirectStandardOutput = True  
      p.StartInfo.FileName = "Write500Lines.exe"  
      p.Start() 

      ' To avoid deadlocks, always read the output stream first and then wait.  
      Dim output As String = p.StandardOutput.ReadToEnd()  
      p.WaitForExit()

      Console.WriteLine($"The last 50 characters in the output stream are:\n'{output.Substring(output.Length - 50)}'")
   End Sub
End Module
' The example displays the following output:
'      Successfully wrote 500 lines.
'
'      The last 50 characters in the output stream are:
'      ' 49,800.20%
'      Line 500 of 500 written: 49,900.20%
'      '