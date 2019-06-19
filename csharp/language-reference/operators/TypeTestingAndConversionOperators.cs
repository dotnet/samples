using System;
using System.Collections.Generic;

namespace operators
{
    public static class TypeTestingAndConversionOperators
    {
        public static void Examples()
        {
            Cast();
            IsOperatorExample.Main();
            IsOperatorWithInt();
            IsOperatorTypePattern();
            AsOperator();
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