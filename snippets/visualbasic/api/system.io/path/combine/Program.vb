Imports System.IO

Module Program
    Public Sub Main()
        Console.WriteLine("Path.Combine(String())")
        Combine1()
        Console.WriteLine()
        Console.WriteLine("Path.Combine(String,String)")
        Combine2()
        Console.WriteLine()
        Console.WriteLine("Path.Combine(String,String,String)")
        Combine3()
        Console.WriteLine()
        Console.WriteLine("Path.Combine(String,String,String,String)")
        Combine4()
    End Sub

    Private Sub Combine1()
        ' <Snippet1>
        Dim paths As String() = { "d:\archives", "2001", "media", "images" }
        Dim fullPath As String = Path.Combine(paths)
        Console.WriteLine(fullPath)            

        paths = { "d:\archives\", "2001\", "media", "images" }
        fullPath = Path.Combine(paths)
        Console.WriteLine(fullPath) 

        paths = { "d:/archives/", "2001/", "media", "images" }
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

    Private Sub Combine2()
        ' <Snippet2>
        Dim result = Path.Combine("C:\Pictures\", "Saved Pictures") 
        Console.WriteLine(result)
        ' The example displays the following output if run on a Windows system:
        '    C:\Pictures\Saved Pictures
        '
        ' The example displays the following output if run on a Unix-based system:
        '    C:\Pictures\/Saved Pictures        
        ' </Snippet2>
    End Sub

    Private Sub Combine3()
        ' <Snippet3>
        Dim result = Path.Combine("C:\Pictures\", "Saved Pictures\", "2019") 
        Console.WriteLine(result)
        ' The example displays the following output if run on a Windows system:
        '    C:\Pictures\Saved Pictures\2019
        '
        ' The example displays the following output if run on a Unix-based system:
        '    C:\Pictures\/Saved Pictures\/2019      
        ' </Snippet3>
    End Sub

   Private Sub Combine4()
        ' <Snippet4>
        Dim result = Path.Combine("C:\Pictures\", "Saved Pictures\", "2019\", "Jan\") 
        Console.WriteLine(result)
        ' The example displays the following output if run on a Windows system:
        '    C:\Pictures\Saved Pictures\2019\Jan\
        '
        ' The example displays the following output if run on a Unix-based system:
        '    C:\Pictures\Saved Pictures\2019\Jan\      
        ' </Snippet4>
    End Sub
End Module
