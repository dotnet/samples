using System;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine($"Path.DirectorySeparatorChar: '{Path.DirectorySeparatorChar}'");
        Console.WriteLine($"Path.AltDirectorySeparatorChar: '{Path.AltDirectorySeparatorChar}'");
        Console.WriteLine($"Path.PathSeparator: '{Path.PathSeparator}'");
        Console.WriteLine($"Path.VolumeSeparatorChar: '{Path.VolumeSeparatorChar}'");
        var invalidChars = Path.GetInvalidPathChars();
        Console.WriteLine($"Path.GetInvalidPathChars:");
        for (int ctr = 0; ctr < invalidChars.Length; ctr++) 
        {
            Console.Write($"  U+{Convert.ToUInt16(invalidChars[ctr]):X4} ");
            if ((ctr + 1) % 10 == 0) Console.WriteLine();
        }
        Console.WriteLine();
    }
}
// The example displays the following output when run on a Windows system:
//    Path.DirectorySeparatorChar: '\'
//    Path.AltDirectorySeparatorChar: '/'
//    Path.PathSeparator: ';'
//    Path.VolumeSeparatorChar: ':'
//    Path.GetInvalidPathChars:
//      U+007C)   U+0000)   U+0001)   U+0002)   U+0003)   U+0004)   U+0005)   U+0006)   U+0007)   U+0008)
//      U+0009)   U+000A)   U+000B)   U+000C)   U+000D)   U+000E)   U+000F)   U+0010)   U+0011)   U+0012)
//      U+0013)   U+0014)   U+0015)   U+0016)   U+0017)   U+0018)   U+0019)   U+001A)   U+001B)   U+001C)
//      U+001D)   U+001E)   U+001F)
//
// The example displays the following output when run on a Linux system:
//    Path.DirectorySeparatorChar: '/'
//    Path.AltDirectorySeparatorChar: '/'
//    Path.PathSeparator: ':'
//    Path.VolumeSeparatorChar: '/'
//    Path.GetInvalidPathChars:
//      U+0000