// <Snippet1>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EnumDir
{
    class Program
    {
        static void Main(string[] args)
        {
        // Set a variable to the My Documents path.
        string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        DirectoryInfo dirPrograms = new DirectoryInfo(@mydocpath);
        DateTime StartOf2009 = new DateTime(2009, 01, 01);

var dirs = from dir in dirPrograms.EnumerateDirectories()
            where dir.CreationTimeUtc > StartOf2009
            select new
            {
                ProgDir = dir,
            };

foreach (var di in dirs)
{
    Console.WriteLine("{0}", di.ProgDir.Name);
}

        }
    }
}
// </Snippet1>
