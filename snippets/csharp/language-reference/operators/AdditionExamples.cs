using System;

namespace operators
{
    public static class AdditionExamples
    {
        public static void Examples()
        {
            NumericAddition();
            StringConcatenation();
            AddDelegates();
            AddAndAssign();
        }

        private static void NumericAddition()
        {
            // <SnippetAddNumerics>
            Console.WriteLine(5 + 4);       // output: 9
            Console.WriteLine(5 + 4.3);     // output: 9.3
            Console.WriteLine(5.1m + 4.2m); // output: 9.3
            // </SnippetAddNumerics>
        }

        private static void StringConcatenation()
        {
            // <SnippetAddStrings>
            Console.WriteLine("Forgot" + "whitespace");
            Console.WriteLine("Probably the oldest constant: " + Math.PI);
            // Output:
            // Forgotwhitespace
            // Probably the oldest constant: 3.14159265358979
            // </SnippetAddStrings>

            // <SnippetUseStringInterpolation>
            Console.WriteLine($"Probably the oldest constant: {Math.PI:F2}");
            // Output:
            // Probably the oldest constant: 3.14
            // </SnippetUseStringInterpolation>
        }

        private static void AddDelegates()
        {
            // <SnippetAddDelegates>
            Action<int> printDouble = (int s) => Console.WriteLine(2 * s);
            Action<int> printTriple = (int s) => Console.WriteLine(3 * s);
            Action<int> combined = printDouble + printTriple;
            combined(5);
            // Output:
            // 10
            // 15
            // </SnippetAddDelegates>
        }

        private static void AddAndAssign()
        {
            // <SnippetAddAndAssign>
            int a = 5;
            a += 9;
            Console.WriteLine(a);
            // Output: 14

            string story = "Start. ";
            story += "End.";
            Console.WriteLine(story);
            // Output: Start. End.

            Action<int> printer = (int s) => Console.WriteLine(s);
            printer += (int s) => Console.WriteLine(2 * s);
            printer(3);
            // Output:
            // 3
            // 6
            // </SnippetAddAndAssign>
        }
    }
}
