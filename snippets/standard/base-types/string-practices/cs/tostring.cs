using System;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        // <Snippet2>
        string value = "The amount is " + 126.03.ToString(CultureInfo.InvariantCulture) + ".";
        Console.WriteLine(value);
        // </Snippet2>

        // <Snippet1>
        value = "The amount is " + 126.03 + ".";
        Console.WriteLine(value);
        // </Snippet1>
    }
}
