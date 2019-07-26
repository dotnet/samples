Imports System.IO

Public Module Example
   Public Sub Main()
      For ctr As Integer = 0 To 499
         Console.WriteLine($"Line {ctr + 1} of 500 written: {ctr + 1/500.0:P2}")
      Next

      Console.Error.WriteLine($"{vbCrLf}Successfully wrote 500 lines.{vbCrLf}")
   End Sub
End Module
' The example displays the following output:
'      The last 50 characters in the output stream are:
'      ' 49,800.20%
'      Line 500 of 500 written: 49,900.20%
'
'
'      Error stream: Successfully wrote 500 lines.