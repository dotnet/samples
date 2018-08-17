using System;

namespace nullable_types
{
    class NullableTypesUsage
    {
        internal static void Examples()
        {
            ExamineValueOfNullableType();
            UseNullCoalescingOperator();
            ComparisonOperators();
            BoxingAndUnboxing();
        }

        private static void DeclareAndAssign()
        {
            // <Snippet1>
            double? pi = 3.14;
            char? letter = 'a';

            int m2 = 10;
            int? m = m2;

            bool? flag = null;
            
            // Array of nullable type:
            int?[] arr = new int?[10];
            // </Snippet1>
        }

        private static void ExamineValueOfNullableType()
        {
            // <Snippet2>
            int? x = 10;
            if (x.HasValue)
            {
                Console.WriteLine($"x is {x.Value}");
            }
            else
            {
                Console.WriteLine("x does not have a value");
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
                Console.WriteLine("y does not have a value");
            }
            // </Snippet3>

            // <Snippet4>
            int? z = 42;
            if (z is int valueOfZ)
            {
                Console.WriteLine($"z is {valueOfZ}");
            }
            else
            {
                Console.WriteLine("z does not have a value");
            }
            // </Snippet4>
        }

        private static void UseNullCoalescingOperator()
        {
            // <Snippet5>
            int? c = null;

            // d = c, if c is not null, d = -1 if c is null.
            int d = c ?? -1;
            Console.WriteLine($"d is {d}");
            // </Snippet5>
        }

        private static void ExplicitCast()
        {
            // <Snippet6>
            int? n = null;

            //int m1 = n;    // Doesn't compile.
            int n2 = (int)n; // Compiles, but throws an exception if n is null.
            // </Snippet6>
        }

        private static void Operators()
        {
            // <Snippet7>
            int? a = 10;
            int? b = null;
            int? c = 10;

            a++;        // a is 11.
            a = a * c;  // a is 110.
            a = a + b;  // a is null.
            // </Snippet7>
        }

        private static void ComparisonOperators()
        {
            // <Snippet8>
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
            // </Snippet8>
        }

        private static void BoxingAndUnboxing()
        {
            // <Snippet9>
            int a = 41;
            object aBoxed = a;
            int? aNullable = (int?)aBoxed;
            Console.WriteLine($"Value of aNullable: {aNullable}");

            object aNullableBoxed = aNullable;
            if (aNullableBoxed is int valueOfA)
            {
                Console.WriteLine($"aNullableBoxed is boxed int: {valueOfA}");
            }
            // Output:
            // Value of aNullable: 41
            // aNullableBoxed is boxed int: 41
            // </Snippet9>
        }
    }
}
