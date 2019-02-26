using System;

namespace operators
{
    public static class MultiplicationExamples
    {
        public static void Examples()
        {
            Multiply();
            MultiplyAndAssign();
        }

        private static void Multiply()
        {
            // <SnippetMultiply>
            Console.WriteLine(5 * 2);         // output: 10
            Console.WriteLine(0.5 * 2.5);     // output: 1.25
            Console.WriteLine(0.1m * 23.4m);  // output: 2.34
            // </SnippetMultiply>
        }

        private static void MultiplyAndAssign()
        {
            // <SnippetMultiplyAndAssign>
            double a = 0.5;
            a *= 3.5;
            Console.WriteLine(a);  // output: 1.75
            // </SnippetMultiplyAndAssign>
        }
    }
}