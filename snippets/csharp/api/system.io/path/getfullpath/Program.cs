using System;
using System.IO;

class Program
{
    static void Main()
    {
        string basePath = Environment.CurrentDirectory;
        string relativePath = "./data/output.xml";
 
        // Unexpectedly change the current directory.
        Environment.CurrentDirectory = "C:/Users/Public/Documents/";
        
        string fullPath = Path.GetFullPath(relativePath, basePath);
        Console.WriteLine($"Current directory:\n   {Environment.CurrentDirectory}");
        Console.WriteLine($"Fully qualified path:\n   {fullPath}");
    }
}
// The example displays the following output:
//   Current directory:
//      C:\Users\Public\Documents
//   Fully qualified path:
//      C:\Utilities\data\output.xml