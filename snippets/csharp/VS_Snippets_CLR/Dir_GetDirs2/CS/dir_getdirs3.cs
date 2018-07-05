// <Snippet2>
using System;
using System.IO;

class Test 
{
    public static void Main() 
    {
            string[] dirs = Directory.GetDirectories(@"c:\", "p*", SearchOption.TopDirectoryOnly);
            Console.WriteLine("The number of directories starting with p is {0}.", dirs.Length);
            foreach (string dir in dirs) 
            {
                Console.WriteLine(dir);
            }
    }
}
// </Snippet2>
