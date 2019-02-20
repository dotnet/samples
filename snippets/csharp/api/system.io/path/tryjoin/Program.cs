using System;
using System.IO;

class Program
{
    static void Main()
    {
        int nChars = 0;
        var buffer = new Span<Char>(new String(' ', 100).ToCharArray());
        var flag = Path.TryJoin("C:/".AsSpan(), "Users/user1".AsSpan(), buffer, out nChars);
        if (flag) 
            Console.WriteLine($"Wrote {nChars} characters: '{buffer.Slice(0, nChars).ToString()}'");
        else
            Console.WriteLine("Concatenation operation failed.");
    }
}
