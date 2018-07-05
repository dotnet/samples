//<snippet3>
using System;
using System.IO;

public class ProcessFile
{
    public static void Main()
    {
        using (StreamReader sr = File.OpenText("data.txt"))
        {
            Console.WriteLine("The first line of this file is {0}", sr.ReadLine());
        }
    }
}
//</snippet3>
