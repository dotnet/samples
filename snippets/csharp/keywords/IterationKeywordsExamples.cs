using System;
using System.Collections.Generic;

namespace keywords
{
    public static class IterationKeywordsExamples
    {
        public static void Examples()
        {
            ForeachWithIEnumerable();
            ForeachWithSpan();
            WhileLoopExample();
            DoLoopExample();
            TypicalForLoopExample();
            ForLoopWithSeveralStatementsInSectionsExample();
        }

        private static void ForeachWithIEnumerable()
        {
            // <Snippet1>
            var fibNumbers = new List<int> { 0, 1, 1, 2, 3, 5, 8, 13 };
            int count = 0;
            foreach (int element in fibNumbers)
            {
                count++;
                Console.WriteLine($"Element #{count}: {element}");
            }
            Console.WriteLine($"Number of elements: {count}");
            // </Snippet1>
        }

        private static void ForeachWithSpan()
        {
            // <Snippet2>
            Span<int> numbers = new int[] { 3, 14, 15, 92, 6 };
            foreach (int number in numbers)
            {
                Console.Write($"{number} ");
            }
            Console.WriteLine();
            // </Snippet2>
        }

        private static void WhileLoopExample()
        {
            // <Snippet3>
            int n = 0;
            while (n < 5)
            {
                Console.WriteLine(n);
                n++;
            }
            // </Snippet3>
        }

        private static void DoLoopExample()
        {
            // <Snippet4>
            int n = 0;
            do 
            {
                Console.WriteLine(n);
                n++;
            } while (n < 5);
            // </Snippet4>
        }

        private static void TypicalForLoopExample()
        {
            // <Snippet5>
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
            }
            // </Snippet5>
        }

        private static void ForLoopWithSeveralStatementsInSectionsExample()
        {
            // <Snippet6>
            int i;
            int j = 10;
            for (i = 0, Console.WriteLine($"Start: i={i}, j={j}"); i < j; i++, j--, Console.WriteLine($"Step: i={i}, j={j}"))
            {
                // Body of the loop.
            }
            // </Snippet6>
        }

        private static void InfiniteForLoopExample()
        {
            // <Snippet7>
            for ( ; ; )
            {
                // Body of the loop.
            }
            // </Snippet7>
        }
    }
}
