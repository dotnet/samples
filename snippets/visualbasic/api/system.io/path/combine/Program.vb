Imports System
Imports System.IO

Module Program
    Public Sub Main()
        Console.WriteLine("Path.Combine(String())")
        Combine1()
    End Sub

    Private Sub Combine1()
        ' <Snippet1>
        Dim paths As String() = {"d:\archives", "2001", "media", "images"}
        Dim fullPath As String = Path.Combine(paths)
        Console.WriteLine(fullPath)            

        paths = {"d:\archives\", "2001\", "media", "images"}
        fullPath = Path.Combine(paths)
        Console.WriteLine(fullPath) 

        paths = {"d:/archives/", "2001/", "media", "images"}
        fullPath = Path.Combine(paths)
        Console.WriteLine(fullPath) 
        ' The example displays the following output if run on a Windows system:
        '    d:\archives\2001\media\images
        '    d:\archives\2001\media\images
        '    d:/archives/2001/media\images
        '
        ' The example displays the following output if run on a Linux system:
        '    d:\archives/2001/media/images
        '    d:\archives\/2001\/media/images
        '    d:/archives/2001/media/images    
        ' </Snippet1>
    End Sub
End Module
