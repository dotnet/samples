using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Path.Combine(String[])");
        Combine1();
        Console.WriteLine("\nPath.Combine(String,String)");
        Combine2();
        Console.WriteLine("\nPath.Combine(String,String,String)");
        Combine3();
        Console.WriteLine("\nPath.Combine(String,String,String,String)");
        Combine4();
    }

    static void Combine1()
    {
        // <Snippet1>
        string[] paths = {@"d:\archives", "2001", "media", "images"};
        string fullPath = Path.Combine(paths);
        Console.WriteLine(fullPath);            

        paths = new string[] {@"d:\archives\", @"2001\", "media", "images"};
        fullPath = Path.Combine(paths);
        Console.WriteLine(fullPath); 

        paths = new string[] {"d:/archives/", "2001/", "media", "images"};
        fullPath = Path.Combine(paths);
        Console.WriteLine(fullPath); 
        // The example displays the following output if run on a Windows system:
        //    d:\archives\2001\media\images
        //    d:\archives\2001\media\images
        //    d:/archives/2001/media\images
        //
        // The example displays the following output if run on a Unix-based system:
        //    d:\archives/2001/media/images
        //    d:\archives\/2001\/media/images
        //    d:/archives/2001/media/images        
        // </Snippet1>
    }

    private static void Combine2()
    {
        // <Snippet2>
        var result = Path.Combine(@"C:\Pictures\", "Saved Pictures"); 
        Console.WriteLine(result);
        // The example displays the following output if run on a Windows system:
        //    C:\Pictures\Saved Pictures
        //
        // The example displays the following output if run on a Unix-based system:
        //    C:\Pictures\/Saved Pictures        
        // </Snippet2>
    }

    private static void Combine3()
    {
        // <Snippet3>
        var result = Path.Combine(@"C:\Pictures\", @"Saved Pictures\", "2019"); 
        Console.WriteLine(result);
        // The example displays the following output if run on a Windows system:
        //    C:\Pictures\Saved Pictures\2019
        //
        // The example displays the following output if run on a Unix-based system:
        //    C:\Pictures\/Saved Pictures\/2019      
        // </Snippet3>
    }

   private static void Combine4()
    {
        // <Snippet4>
        var result = Path.Combine(@"C:\Pictures\", @"Saved Pictures\", @"2019\", @"Jan\"); 
        Console.WriteLine(result);
        // The example displays the following output if run on a Windows system:
        //    C:\Pictures\Saved Pictures\2019\Jan\
        //
        // The example displays the following output if run on a Unix-based system:
        //    C:\Pictures\Saved Pictures\2019\Jan\      
        // </Snippet4>
    }
}
