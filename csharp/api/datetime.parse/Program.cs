using System;
using System.Collections.Generic;

class ParseExamples
{
    private static readonly KeyValuePair<string, Action>[] examples;
    private static string msg = null;

    static ParseExamples()
    {
        examples = new KeyValuePair<string, Action>[] {
            new KeyValuePair<string, Action>(" 1. Forms of the string to be parsed", Strings.Parse),
            new KeyValuePair<string, Action>(" 2. Return value: The DateTime.Kind property", ReturnValue.Kind),
            new KeyValuePair<string, Action>(" 3. StyleFlags.RoundtripKind: Round-tripping a DateTime value", StyleFlag.RoundtripKind),
            new KeyValuePair<string, Action>(" 4. DateTime.Parse(String) overload", DateTimeParse1.ParseWithSingleArg),
            new KeyValuePair<string, Action>(" 5. DateTime.Parse(String, IFormatProvider) overload", DateTimeParse2.ParseWithTwoArgs),
            new KeyValuePair<string, Action>(" 6. DateTime.Parse(String, IFormatProvider, DateTimeStyles) overload", DateTimeParse3.ParseWithThreeArgs) };
    }

    static void Main()
    {
        do
        {
            var choice = GetSelection(msg);

            // Make sure this parses.
            bool success = Int32.TryParse(choice, out var nChoice);
            msg = "";

            if (!success)
            {
                msg = $"'{choice}' is not a number between 0 and {examples.Length}.";
            }
            else
            {
                if (nChoice == 0)
                {
                    return;
                }
                else if (nChoice < 0 || nChoice > examples.Length)
                {
                    msg = $"Your selection must be between 0 and {examples.Length}.";
                }
                else
                {
                    Console.WriteLine();
                    examples[--nChoice].Value();

                    Console.Write("\nPress any key to continue...");
                    Console.ReadKey(false);
                }

            }
        } while (true);
    }

    private static string GetSelection(string msg)
    {
        Console.Clear();
        Console.WriteLine();
        foreach (var example in examples)
            Console.WriteLine(example.Key);

        if (!String.IsNullOrEmpty(msg))
            Console.WriteLine($"\n** {msg} **\n");

        Console.Write("\nEnter the number of the example you wish to run and then press <Enter>. Or, press 0 to exit. ");
        var choice = Console.ReadLine();

        return choice;
    }
}