Imports System.Runtime.CompilerServices
Imports System.Text

Module Program
    Sub Main()
        PerformStringOperation()
        Console.WriteLine("---")
        UseMutableBuffer()
        Console.WriteLine("---")
    End Sub

    Private Sub PerformStringOperation()
        ' <Snippet1>
        Dim original = "This is a sentence. This is a second sentence."
        Dim sentence1 = original.Substring(0, original.IndexOf(".") + 1)
        Console.WriteLine(original)
        Console.WriteLine(sentence1)
        ' The example displays the following output:
        '    This is a sentence. This is a second sentence.
        '    This is a sentence.            
        ' </Snippet1>
    End Sub

    ' <Snippet2>
    Private Sub UseMutableBuffer()
        Dim original = "This is a sentence. This is a second sentence."
        Dim sb = new StringBuilder(original)
        Dim index = original.IndexOf(".")
        sb(index) = ";"
        sb(index + 2) = Char.ToLower(sb(index + 2))
        Console.WriteLine($"Original string: {original}")
        Console.WriteLine($"Modified string: {sb.ToString()}")
    End Sub
    ' The example displays the following output:
    '    Original string: This is a sentence. This is a second sentence.
    '    Modified string: This is a sentence; this is a second sentence.        
    ' </Snippet2>
End Module
