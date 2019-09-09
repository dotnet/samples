using System;
using System.Collections.Generic;
using System.Linq;

namespace operators
{
    public static class NullCoalescingOperator
    {
        public static void Examples()
        {
            WithNullConditional();
            WithNullableTypes();
            NullCoalescingAssignment();
            TypeOfNullCoalescingAssignment();
        }

        private static void WithNullConditional()
        {
            // <SnippetWithNullConditional>
            double SumNumbers(List<double[]> setsOfNumbers, int indexOfSetToSum)
            {
                return setsOfNumbers?[indexOfSetToSum]?.Sum() ?? double.NaN;
            }

            var sum = SumNumbers(null, 0);
            Console.WriteLine(sum);  // output: NaN
            // </SnippetWithNullConditional>
        }

        private static void WithNullableTypes()
        {
            // <SnippetWithNullableTypes>
            int? a = null;
            int b = a ?? -1;
            Console.WriteLine(b);  // output: -1
            // </SnippetWithNullableTypes>
        }

        private class Person
        {
            string name;

            // <SnippetWithThrowExpression>
            public string Name
            {
                get => name;
                set => name = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null");
            }
            // </SnippetWithThrowExpression>
        }

        // <SnippetUnconstrainedType>
        private static void Display<T>(T a, T backup)
        {
            Console.WriteLine(a ?? backup);
        }
        // </SnippetUnconstrainedType>

        private static void NullCoalescingAssignment()
        {
            // <SnippetAssignment>
            List<int> InitializeList()
            {
                Console.WriteLine("Initializing list...");
                return new List<int>();
            }

            List<int> numbers = null;
            (numbers ??= InitializeList()).Add(5);
            Console.WriteLine(string.Join(" ", numbers));

            numbers ??= InitializeList();
            Console.WriteLine(string.Join(" ", numbers));
            // Output:
            // Initializing list...
            // 5
            // 5
            // </SnippetAssignment>
        }

        private static void TypeOfNullCoalescingAssignment()
        {
            // <SnippetTypeOfResult>
            int? a = null;
            int b = 6;
            int c = (a ??= b);
            Console.WriteLine(c);  // output: 6
            // </SnippetTypeOfResult>
        }
    }
}