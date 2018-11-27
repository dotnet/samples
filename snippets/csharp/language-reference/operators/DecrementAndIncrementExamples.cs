using System;

namespace operators
{
    public static class DecrementAndIncrementExamples
    {
        public static void Examples()
        {
            Decrement();
            Increment();
        }

        private static void Decrement()
        {
            // <SnippetPrefixDecrement>
            double a = 1.5;
            Console.WriteLine(a);   // output: 1.5
            Console.WriteLine(--a); // output: 0.5
            Console.WriteLine(a);   // output: 0.5
            // </SnippetPrefixDecrement>

            // <SnippetPostfixDecrement>
            int i = 3;
            Console.WriteLine(i);   // output: 3
            Console.WriteLine(i--); // output: 3
            Console.WriteLine(i);   // output: 2
            // </SnippetPostfixDecrement>
        }

        private static void Increment()
        {
            // <SnippetPrefixIncrement>
            double a = 1.5;
            Console.WriteLine(a);   // output: 1.5
            Console.WriteLine(++a); // output: 2.5
            Console.WriteLine(a);   // output: 2.5
            // </SnippetPrefixIncrement>

            // <SnippetPostfixIncrement>
            int i = 3;
            Console.WriteLine(i);   // output: 3
            Console.WriteLine(i++); // output: 3
            Console.WriteLine(i);   // output: 4
            // </SnippetPostfixIncrement>
        }
    }
}