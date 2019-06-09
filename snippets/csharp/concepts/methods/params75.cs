//<Snippet75>
using System;
using System.Linq;

class Example
{
    static void Main()
    {
        string fromArray = GetVowels(
            new[] { "apple", "banana", "pear" });

        string fromParameters = GetVowels(
            "apple", "banana", "pear");

        string fromSingleValue = GetVowels(
            "apple, banana, pear");

        Console.WriteLine("Vowels in words:");
        Console.WriteLine($"    From array: {fromArray}");
        Console.WriteLine($"    From parameters: {fromParameters}");
        Console.WriteLine($"    From single value: {fromSingleValue}");
    }

    static string GetVowels(params string[] input)
    {
        if (input == null || input.Length == 0)
        {
            return string.Empty;
        }

        var vowels = new char[] { 'A', 'E', 'I', 'O', 'U' };
        return string.Concat(
            input.SelectMany(
                word => word.Where(letter => vowels.Contains(char.ToUpper(letter)))));
    }
}

// The example displays the following output:
//     Vowels in words:
//         From array: aeaaaea
//         From parameters: aeaaaea
//         From single value: aeaaaea

//<Snippet75>