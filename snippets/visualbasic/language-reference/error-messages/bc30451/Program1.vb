Imports Microsoft.VisualBasic.CompilerServices

Public Module Example
   Sub Main(args As String())
        Dim originalValue = args(0)
        Dim t = GetType(Int32)
        Dim i = Conversions.ChangeType(originalValue, t)
        Console.WriteLine($"'{originalValue}' --> {i}")
    End Sub
End Module
