using System;
using System.IO;

class Program
{
    static void Main()
    {
        string relative1 = "C:Documents"; 
        ShowPathInfo(relative1);

        string relative2 = "/Documents";
        ShowPathInfo(relative2);

        string absolute = "C:/Documents";
        ShowPathInfo(absolute);
    }

    private static void ShowPathInfo(string path)
    {
        Console.WriteLine($"Path: {path}");
        Console.WriteLine($"   Rooted: {Path.IsPathRooted(path)}");
        Console.WriteLine($"   Fully qualified: {Path.IsPathFullyQualified(path)}");
        Console.WriteLine($"   Full path: {Path.GetFullPath(path)}");
        Console.WriteLine();
    }
}
// The example displays the following output when run on a Windows system:
//    Path: C:Documents
//        Rooted: True
//        Fully qualified: False
//        Full path: c:\Users\user1\Documents\projects\path\ispathrooted\Documents
//
//    Path: /Documents
//       Rooted: True
//       Fully qualified: False
//       Full path: c:\Documents
//
//    Path: C:/Documents
//       Rooted: True
//       Fully qualified: True
//       Full path: C:\Documents