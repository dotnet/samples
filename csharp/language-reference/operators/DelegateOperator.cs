using System;
using System.Collections.Generic;

namespace operators
{
    public static class DelegateOperator
    {
        public static void Examples()
        {
            AnonymousMethod();
            Lambda();
            WithoutParameterList();
        }

        private static void AnonymousMethod()
        {
            // <SnippetAnonymousMethod>
            Action<IEnumerable<int>> display = delegate (IEnumerable<int> s)
            {
                Console.WriteLine(string.Join(" ", s));
            };

            var numbers = new[] { 1, 2, 3 };
            display(numbers);  // output: 1 2 3
            // </SnippetAnonymousMethod>
        }

        private static void Lambda()
        {
            // <SnippetLambda>
            Action<IEnumerable<int>> display = s => Console.WriteLine(string.Join(" ", s));

            var numbers = new[] { 1, 2, 3 };
            display(numbers);  // output 1 2 3
            // </SnippetLambda>
        }

        private static void WithoutParameterList()
        {
            // <SnippetWithoutParameterList>
            Action greet = delegate { Console.WriteLine("Hello!"); };
            greet();
            
            Action<int, double> introduce = delegate { Console.WriteLine("This is world!"); };
            introduce(42, 2.7);

            // Output:
            // Hello!
            // This is world!
            // </SnippetWithoutParameterList>
        }
    }
}