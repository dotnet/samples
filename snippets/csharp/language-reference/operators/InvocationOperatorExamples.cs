using System;
using System.Collections.Generic;

namespace operators
{
    public static class InvocationOperatorExamples
    {
        public static void Examples()
        {
            Invocation();
            Cast();
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

        private static void Cast()
        {
            // <SnippetCast>
            double x = 1234.7;
            int a = (int)x;
            Console.WriteLine(a);   // output: 1234
            // </SnippetCast>
        }
    }
}