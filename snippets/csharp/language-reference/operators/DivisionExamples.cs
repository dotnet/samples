using System;

namespace operators
{
    public static class DivisionExamples
    {
        public static void Examples()
        {
            IntegerDivision();
            IntegerAsFloatingPointDivision();
            FloatingPointDivision();
            DivisionAssignment();
        }

        private static void IntegerDivision()
        {
            // <SnippetInteger>
            Console.WriteLine(13 / 5);    // output: 2
            Console.WriteLine(-13 / 5);   // output: -2
            Console.WriteLine(13 / -5);   // output: -2
            Console.WriteLine(-13 / -5);  // output: 2
            // </SnippetInteger>
        }

        private static void IntegerAsFloatingPointDivision()
        {
            // <SnippetIntegerAsFloatingPoint>
            Console.WriteLine(13 / 5.0);       // output: 2.6

            int a = 13;
            int b = 5;
            Console.WriteLine(a / (double)b);  // output: 2.6
            // </SnippetIntegerAsFloatingPoint>
        }

        private static void FloatingPointDivision()
        {
            // <SnippetFloatingPoint>
            Console.WriteLine(16.8f / 4.1f);
            Console.WriteLine(16.8d / 4.1d);
            Console.WriteLine(16.8m / 4.1m);
            // Output:
            // 4.097561
            // 4.09756097560976
            // 4.0975609756097560975609756098
            // </SnippetFloatingPoint>
        }

        private static void DivisionAssignment()
        {
            // <SnippetDivisionAssignment>
            int a = 4;
            int b = 5;
            a /= b;
            Console.WriteLine(a);   // output: 0

            double x = 4;
            double y = 5;
            x /= y;
            Console.WriteLine(x);   // output: 0.8
            // </SnippetDivisionAssignment>
        }
    }
}