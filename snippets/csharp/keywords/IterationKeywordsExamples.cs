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
    }
}
