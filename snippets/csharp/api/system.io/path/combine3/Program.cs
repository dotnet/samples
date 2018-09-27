using System;
using System.IO;

class Program
{
    static void Main()
    {
        string[] paths1 = { @"C:\", @"C:\", @"\Program Files", "Program Files" };
        string[] paths2 = { @"\users\user1\documents", @"D:\", @"\Utilities", @"D:\Utilities" };
        string[] paths3 = { @"\letters\", @"E:", @"\FileFinder\", @"E:\Temp\" };

        for(int ctr = 0; ctr < paths1.Length; ctr++)
        {
            Console.WriteLine($"\nConcatenating '{paths1[ctr]}', '{paths2[ctr]}', and '{paths3[ctr]}'");               
            string combinedPath = Path.Combine(paths1[ctr], paths2[ctr], paths3[ctr]);
            Console.WriteLine($"   Combine:  {combinedPath}");
            Console.WriteLine($"   Full Path: {Path.GetFullPath(combinedPath)}");
            string joinedPath = Path.Join(paths1[ctr], paths2[ctr], paths3[ctr]); 
            Console.WriteLine($"\n   Join:    {joinedPath}");
            Console.WriteLine($"   Full Path: {Path.GetFullPath(joinedPath)}");
        }
    }
}
// The example displays the following output:
//       Concatenating 'C:\', '\users\user1\documents', and '\letters\'
//       Combine:  \letters\
//       Full Path: C:\letters\
//
//       Join:    C:\\users\user1\documents\letters\
//       Full Path: C:\users\user1\documents\letters\
//
//       Concatenating 'C:\', 'D:\', and 'E:'
//       Combine:  E:
//       Full Path: E:\
//
//       Join:    C:\D:\E:
//       Full Path: C:\D:\E:
//
//       Concatenating '\Program Files', '\Utilities', and '\FileFinder\'
//       Combine:  \FileFinder\
//       Full Path: C:\FileFinder\
//
//       Join:    \Program Files\Utilities\FileFinder\
//       Full Path: C:\Program Files\Utilities\FileFinder\
//
//       Concatenating 'Program Files', 'D:\Utilities', and 'E:\Temp\'
//       Combine:  E:\Temp\
//       Full Path: E:\Temp\
//
//       Join:    Program Files\D:\Utilities\E:\Temp\
//       Full Path: C:\Users\rpetrusha\Documents\programs\path\combine2\Program Files\D:\Utilities\E:\Temp\       