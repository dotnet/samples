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
    }
}