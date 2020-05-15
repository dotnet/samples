using System;
using System.Collections.Generic;

class FormatExamples
{
    private static readonly KeyValuePair<string, Action>[] examples;
    private static string msg = null;

    static FormatExamples()
    {
        examples = new KeyValuePair<string, Action>[] {
          new KeyValuePair<string, Action>(" 1. Get Started: Insert an object into a string", GetStarted.Example1),
          new KeyValuePair<string, Action>(" 2. Get Started: Inserting a formatted object into a string", GetStarted.Example2),
          new KeyValuePair<string, Action>(" 3. Get Started: Format item and object", GetStarted.Example3),
          new KeyValuePair<string, Action>(" 4. Get Started: 2 format items and 2 objects", GetStarted.Example4),
          new KeyValuePair<string, Action>(" 5. Get Started: Control formatting", GetStarted.Example5),
          new KeyValuePair<string, Action>(" 6. Get Started: Control spacing", GetStarted.Example6),
          new KeyValuePair<string, Action>(" 7: Get Started: Control alignment", GetStarted.Example7),
          new KeyValuePair<string, Action>(" 8: Format Elements: in brief", FormatElements.FormatMethod),
          new KeyValuePair<string, Action>(" 9: Format Elements: format item", FormatElements.FormatItem),
          new KeyValuePair<string, Action>("10: Format Elements: Formatted format item", FormatElements.FormattedFormatItem),
          new KeyValuePair<string, Action>("11: Format Elements: Format items with the same index", FormatElements.SameIndex),
          new KeyValuePair<string, Action>("12: Custom Formatting: Customer account formatter", CustomerFormatterTest.Test),
          new KeyValuePair<string, Action>("13: Custom Formatting: Roman numeral formatter", RomanNumeralExample.Test),
          new KeyValuePair<string, Action>("14: Q & A: String interpolation comparison", QA.WithoutInterpolation),
          new KeyValuePair<string, Action>("15: Q & A: Digits after the decimal separator", QA.DecimalDigits),
          new KeyValuePair<string, Action>("16: Q & A: Digits after the decimal separator with custom format string",
                    QA.DigitsUsingCustomFormatSpecifier),
          new KeyValuePair<string, Action>("17: Q & A: Integral digits", QA.IntegralDigits),
          new KeyValuePair<string, Action>("18: Q & A: Integral digits with a custom format string", QA.IntegralDigitsUsingCustom),
          new KeyValuePair<string, Action>("19: Q & A: Escaped braces", QA.EscapedBraces),
          new KeyValuePair<string, Action>("20: Q & A: Braces in a format list", QA.BracesInFormatList),
          new KeyValuePair<string, Action>("21: Q & A: Parameter Arrays and FormatExceptions", QA.FormatException),
          new KeyValuePair<string, Action>("22: Examples: Format a single argument", Examples.SingleArgument),
          new KeyValuePair<string, Action>("23: Examples: Format two arguments", Examples.TwoArguments),
          new KeyValuePair<string, Action>("24: Examples: Format three arguments", Examples.ThreeArguments),
          new KeyValuePair<string, Action>("25: Examples: Format more than three arguments #1", Examples.MoreThanThree_1),
          new KeyValuePair<string, Action>("26: Examples: Format more than three arguments #2", Examples.MoreThanThree_2),
          new KeyValuePair<string, Action>("27: Examples: Culture-sensitive formatting", Examples.CultureSensitive) };
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

        Console.Write("\nEnter the number of the example you wish to run, and then press <Enter>. Or, press 0 to exit. ");
        var choice = Console.ReadLine();

        return choice;
    }
}