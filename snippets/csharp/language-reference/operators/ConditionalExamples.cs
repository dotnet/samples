using System;
using System.Collections.Generic;
using System.Text;

namespace operators
{
    public class ConditionalExamples
    {
        public static void Examples()
        {
            ConditionalValueExpressions();
        }

        static void ConditionalRefExpressions()
        {
            // <SnippetConditionalRef>
            var smallArray = new int[] { 1, 2, 3, 4, 5 };
            var largeArray = new int[] { 10, 20, 30, 40, 50 };

            int index= new Random().Next(0, 9);
            ref int refValue = ref ((index < 5) ? ref smallArray[index] : ref largeArray[index - 5]);
            Console.WriteLine(refValue);
            // </SnippetConditionalRef>
        }


        static void ConditionalValueExpressions()
        {
            // <SnippetConditionalValue>
            double sinc(double x) =>
                x != 0.0 ? Math.Sin(x) / x : 1.0;

            Console.WriteLine(sinc(0.2));
            Console.WriteLine(sinc(0.1));
            Console.WriteLine(sinc(0.0));
            /*
            Output:
            0.993346653975306
            0.998334166468282
            1
            */
            // </SnippetConditionalValue>
        }
    }
}
