using System;
using System.Collections.Generic;

namespace operators
{
    public static class TypeTestingAndAssignmentOperators
    {
        public static void Examples()
        {
            Assignment();
            RefAssignment();
            Cast();
            IsOperatorExample.Main();
            IsOperatorWithInt();
            IsOperatorTypePattern();
            AsOperator();
        }

        private static void Assignment()
        {
            // <SnippetAssignment>
            var numbers = new List<double>() { 1.0, 2.0, 3.0 };

            Console.WriteLine(numbers.Capacity);
            numbers.Capacity = 100;
            Console.WriteLine(numbers.Capacity);
            // Output:
            // 4
            // 100

            int newFirstElement;
            double originalFirstElement = numbers[0];
            newFirstElement = 5;
            numbers[0] = newFirstElement;
            Console.WriteLine(originalFirstElement);
            Console.WriteLine(numbers[0]);
            // Output:
            // 1
            // 5
            // </SnippetAssignment>
        }

        private static void RefAssignment()
        {
            // <SnippetRefAssignment>
            void Display(double[] s) => Console.WriteLine(string.Join(" ", s));
            
            double[] arr = { 0.0, 0.0, 0.0 };
            Display(arr);

            ref double arrayElement = ref arr[0];
            arrayElement = 3.0;
            Display(arr);

            arrayElement = ref arr[arr.Length - 1];
            arrayElement = 5.0;
            Display(arr);
            // Output:
            // 0 0 0
            // 3 0 0
            // 3 0 5
            // </SnippetRefAssignment>
        }

        private static void Cast()
        {
            // <SnippetCast>
            double x = 1234.7;
            int a = (int)x;
            Console.WriteLine(a);   // output: 1234

            IEnumerable<int> numbers = new int[] { 10, 20, 30 };
            IList<int> list = (IList<int>)numbers;
            Console.WriteLine(list.Count);  // output: 3
            Console.WriteLine(list[1]);  // output: 20
            // </SnippetCast>
        }

        // <SnippetIsWithReferenceConversion>
        public class Base { }

        public class Derived : Base { }

        public static class IsOperatorExample
        {
            public static void Main()
            {
                object b = new Base();
                Console.WriteLine(b is Base);  // output: True
                Console.WriteLine(b is Derived);  // output: False

                object d = new Derived();
                Console.WriteLine(d is Base);  // output: True
                Console.WriteLine(d is Derived); // output: True
            }
        }
        // </SnippetIsWithReferenceConversion>

        private static void IsOperatorWithInt()
        {
            // <SnippetIsWithInt>
            int i = 27;
            Console.WriteLine(i is System.IFormattable);  // output: True
            
            object iBoxed = i;
            Console.WriteLine(iBoxed is int);  // output: True
            Console.WriteLine(iBoxed is long);  // output: False
            // </SnippetIsWithInt>
        }

        private static void IsOperatorTypePattern()
        {
            // <SnippetIsTypePattern>
            int i = 23;
            object iBoxed = i;
            int? j = 7;
            if (iBoxed is int a && j is int b)
            {
                Console.WriteLine(a + b);  // output 30
            }
            // </SnippetIsTypePattern>
        }

        private static void AsOperator()
        {
            // <SnippetAsOperator>
            IEnumerable<int> numbers = new[] { 10, 20, 30 };
            IList<int> indexableNumbers = numbers as IList<int>;
            if (indexableNumbers != null)
            {
                Console.WriteLine(indexableNumbers[0] + indexableNumbers[indexableNumbers.Count - 1]);  // output: 40
            }
            // </SnippetAsOperator>
        }
    }
}