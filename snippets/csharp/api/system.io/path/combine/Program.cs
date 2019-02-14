using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Path.Combine(String[])");
        Combine1();
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
        // The example displays the following output if run on a Linux system:
        //    d:\archives/2001/media/images
        //    d:\archives\/2001\/media/images
        //    d:/archives/2001/media/images        
        // </Snippet1>
    }
}
