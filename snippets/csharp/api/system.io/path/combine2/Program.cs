using System;
using System.IO;

class Program
{
    static void Main()
    {
        string[] paths1 = { @"C:\", @"C:\", @"\Program Files", "Program Files" };
        string[] paths2 = {  @"\users\user1\documents", @"D:\", @"\Utilities", @"D:\Utilities" };

        for(int ctr = 0; ctr < paths1.Length; ctr++)
        {
            Console.WriteLine($"\nConcatenating '{paths1[ctr]}' and '{paths2[ctr]}'");               
            string combinedPath = Path.Combine(paths1[ctr], paths2[ctr]);
            Console.WriteLine($"   Combine:  {combinedPath}");
            Console.WriteLine($"   Full Path: {Path.GetFullPath(combinedPath)}");
            string joinedPath = Path.Join(paths1[ctr], paths2[ctr]); 
            Console.WriteLine($"\n   Join:    {joinedPath}");
            Console.WriteLine($"   Full Path: {Path.GetFullPath(joinedPath)}");
        }
    }
}
// The example displays the following output:
//      Concatenating 'C:\' and '\users\user1\documents'
//      Combine:  \users\user1\documents
//      Full Path: C:\users\user1\documents
//
//      Join:    C:\\users\user1\documents
//      Full Path: C:\users\user1\documents
//
//      Concatenating 'C:\' and 'D:\'
//      Combine:  D:\
//      Full Path: D:\
//
//      Join:    C:\D:\
//      Full Path: C:\D:\
//
//      Concatenating '\Program Files' and '\Utilities'
//      Combine:  \Utilities
//      Full Path: C:\Utilities
//
//      Join:    \Program Files\Utilities
//      Full Path: C:\Program Files\Utilities
//
//      Concatenating 'Program Files' and 'D:\Utilities'
//      Combine:  D:\Utilities
//      Full Path: D:\Utilities
//
//      Join:    Program Files\D:\Utilities
//      Full Path: C:\Development\cors\path\combine\Program Files\D:\Utilities