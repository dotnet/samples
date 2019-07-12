Imports System.Diagnostics

Public Module Example
   Public Sub Main()
      Dim p As New Process()  
      p.StartInfo.UseShellExecute = False  
      p.StartInfo.RedirectStandardOutput = True  
      Dim eOut As String = Nothing
      p.StartInfo.RedirectStandardError = True
      AddHandler p.ErrorDataReceived, Sub(sender, e) eOut += e.Data 
      p.StartInfo.FileName = "Write500Lines.exe"  
      p.Start()  

      ' To avoid deadlocks, use an asynchronous read operation on at least one of the streams.  
      p.BeginErrorReadLine()
      Dim output As String = p.StandardOutput.ReadToEnd()  
      p.WaitForExit()

      Console.WriteLine($"The last 50 characters in the output stream are:{vbCrLf}'{output.Substring(output.Length - 50)}'")
      Console.WriteLine($"{vbCrLf}Error stream: {eOut}")
   End Sub
End Module
' The example displays the following output:
'      The last 50 characters in the output stream are:
'      ' 49,800.20%
'      Line 500 of 500 written: 49,900.20%
'      '
'
'      Error stream: Successfully wrote 500 lines.