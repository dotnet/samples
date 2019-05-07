using System;
// <SnippetNestedNamespace>
using System.Collections.Generic;
// </SnippetNestedNamespace>
using System.Linq;

namespace operators
{
    public static class MemberAccessOperators
    {
        public static void Examples()
        {
            TypeMemberAccess();
            Arrays();
            Indexers();
            NullConditional();
            Invocation();
        }

        private static void QualifiedName()
        {
            // <SnippetQualifiedName>
            System.Collections.Generic.IEnumerable<int> numbers = new int[] { 1, 2, 3 };
            // </SnippetQualifiedName>
        }

        private static void TypeMemberAccess()
        {
            // <SnippetTypeMemberAccess>
            var constants = new List<double>();
            constants.Add(Math.PI);
            constants.Add(Math.E);
            Console.WriteLine($"{constants.Count} values to show:");
            Console.WriteLine(string.Join(", ", constants));
            // Output:
            // 2 values to show:
            // 3.14159265358979, 2.71828182845905
            // </SnippetTypeMemberAccess>
        }

        private static void Arrays()
        {
            // <SnippetArrays>
            int[] fib = new int[10];
            fib[0] = fib[1] = 1;
            for (int i = 2; i < fib.Length; i++)
            {
                fib[i] = fib[i - 1] + fib[i - 2];
            }
            Console.WriteLine(fib[fib.Length - 1]);  // output: 55

            double[,] matrix = new double[2,2];
            matrix[0,0] = 1.0;
            matrix[0,1] = 2.0;
            matrix[1,0] = matrix[1,1] = 3.0;
            var determinant = matrix[0,0] * matrix[1,1] - matrix[1,0] * matrix[0,1];
            Console.WriteLine(determinant);  // output: -3
            // </SnippetArrays>
        }

        private static void Indexers()
        {
            // <SnippetIndexers>
            var dict = new Dictionary<string, double>();
            dict["one"] = 1;
            dict["pi"] = Math.PI;
            Console.WriteLine(dict["one"] + dict["pi"]);  // output: 4.14159265358979
            // </SnippetIndexers>
        }

        private static void NullConditional()
        {
            // <SnippetNullConditional>
            double SumNumbers(List<double[]> setsOfNumbers, int indexOfSetToSum)
            {
                return setsOfNumbers?[indexOfSetToSum]?.Sum() ?? double.NaN;
            }

            var sum1 = SumNumbers(null, 0);
            Console.WriteLine(sum1);  // output: NaN

            var numberSets = new List<double[]>
            {
                new[] { 1.0, 2.0, 3.0 },
                null
            };

            var sum2 = SumNumbers(numberSets, 0);
            Console.WriteLine(sum2);  // output: 6

            var sum3 = SumNumbers(numberSets, 1);
            Console.WriteLine(sum3);  // output: NaN
            // </SnippetNullConditional>
        }

        private static void Invocation()
        {
            // <SnippetInvocation>
            Action<int> display = s => Console.WriteLine(s);

            var numbers = new List<int>();
            numbers.Add(10);
            numbers.Add(17);
            display(numbers.Count);   // output: 2

            numbers.Clear();
            display(numbers.Count);   // output: 0
            // </SnippetInvocation>
        }
    }
}