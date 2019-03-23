Imports System.IO

Module Example
    Public Sub Main()
        Dim path1 = "C:/Program Files/"
        Dim path2 = "Utilities/SystemUtilities"
        ShowPathInformation(path1, path2)

        path1 = "C:/"
        path2 = "/Program Files"
        ShowPathInformation(path1, path2)

        path1 = "C:/Users/Public/Documents/"
        path2 = "C:/Users/User1/Documents/Financial/"
        ShowPathInformation(path1, path2)
    End Sub

    Private Sub ShowPathInformation(path1 As String, path2 As String)
        Dim result = Path.Join(path1.AsSpan(), path2.AsSpan())
        Console.WriteLine($"Concatenating  '{path1}' and '{path2}'")
        Console.WriteLine($"   Path.Join:     '{result}'")
        Console.WriteLine($"   Path.Combine:  '{Path.Combine(path1, path2)}'")
        Console.WriteLine()
    End Sub
End Module
' The example displays the following output if run on a Windows system:
'    Concatenating  'C:/Program Files/' and 'Utilities/SystemUtilities'
'       Path.Join:     'C:/Program Files/Utilities/SystemUtilities'
'       Path.Combine:  'C:/Program Files/Utilities/SystemUtilities'
'
'    Concatenating  'C:/' and '/Program Files'
'       Path.Join:     'C:'Program Files'
'       Path.Combine:  '/Program Files'
'
'    Concatenating  'C:/Users/Public/Documents/' and 'C:/Users/User1/Documents/Financial/'
'       Path.Join:     'C:/Users/Public/Documents/C:/Users/User1/Documents/Financial/'
'       Path.Combine:  'C:/Users/User1/Documents/Financial/'

