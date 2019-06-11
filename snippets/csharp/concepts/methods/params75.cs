//<Snippet75>
using System;
using System.Linq;

class Example
{
    static void Main()
    {
        // Invoking the method with an array
        string fromArray = GetVowels(
            new[] { "apple", "banana", "pear" });

        // Invoking with a comma-separated list of arguments
        string fromMultipleArguments = GetVowels(
            "apple", "banana", "pear");

        // Invoking without any arguments
        string fromNoValue = GetVowels();

        Console.WriteLine("Vowels in words:");
        Console.WriteLine($"    From array: '{fromArray}'");
        Console.WriteLine($"    From multiple arguments: '{fromMultipleArguments}'");
        Console.WriteLine($"    From no value: '{fromNoValue}'");
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
//         From array: 'aeaaaea'
//         From multiple arguments: 'aeaaaea'
//         From no value: ''

//<Snippet75>
