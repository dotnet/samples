using System;

namespace nullable_types
{
    class NullableTypesUsage
    {
        internal static void Examples()
        {
            AccessValueOfNullableType();
            ComparisonOperators();
            NullCoalescingOperator();
        }

        private static void DeclareAndAssign()
        {
            // <Snippet1>
            int? i = 10;
            double? pi = 3.14;
            char? letter = 'a';
            int?[] arr = new int?[10];
            bool? flag = null;
            // </Snippet1>
        }

        private static void AccessValueOfNullableType()
        {
            // <Snippet2>
            int? x = 10;
            if (x.HasValue)
            {
                Console.WriteLine($"x is {x.Value}");
            }
            else
            {
                Console.WriteLine("x is undefined");
            }
            // </Snippet2>

            // <Snippet3>
            int? y = 7;
            if (y != null)
            {
                Console.WriteLine($"y is {y.Value}");
            }
            else
            {
                Console.WriteLine("y is undefined");
            }
            // </Snippet3>
        }

        private static void Conversions()
        {
            // <Snippet4>
            int? n = null;

            //int m1 = n;      // Doesn't compile.
            int n2 = (int)n;   // Compiles, but throws an exception if n is null.
            int n3 = n.Value;  // Compiles, but throws an exception if n is null.
            // </Snippet4>

            // <Snippet5>
            int? m;
            int m2 = 10;
            m = m2;  // Implicit conversion.
            // </Snippet5>
        }

        private static void Operators()
        {
            // <Snippet6>
            int? a = 10;
            int? b = null;
            int? c = 10;

            a++;        // a is 11.
            a = a * c;  // a is 110.
            a = a + b;  // a is null.
            // </Snippet6>
        }

        private static void ComparisonOperators()
        {
            // <Snippet7>
            int? num1 = 10;
            int? num2 = null;
            if (num1 >= num2)
            {
                Console.WriteLine("num1 is greater than or equal to num2");
            }
            else
            {
                Console.WriteLine("num1 >= num2 is false (but num1 < num2 also is false)");
            }

            if (num1 < num2)
            {
                Console.WriteLine("num1 is less than num2");
            }
            else
            {
                Console.WriteLine("num1 < num2 is false (but num1 >= num2 also is false)");
            }

            if (num1 != num2)
            {
                Console.WriteLine("num1 != num2 is true!");
            }

            num1 = null;
            if (num1 == num2)
            {
                Console.WriteLine("num1 == num2 is true if the value of each is null");
            }
            // Output:
            // num1 >= num2 is false (but num1 < num2 also is false)
            // num1 < num2 is false (but num1 >= num2 also is false)
            // num1 != num2 is true!
            // num1 == num2 is true if the value of each is null
            // </Snippet7>
        }

        private static void NullCoalescingOperator()
        {
            // <Snippet8>
            int? c = null;

            // d = c, unless c is null, in which case d = -1.
            int d = c ?? -1;
            Console.WriteLine($"d is {d}");
            // </Snippet8>
        }
    }
}