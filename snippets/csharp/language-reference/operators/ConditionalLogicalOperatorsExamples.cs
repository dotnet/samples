using System;

namespace operators
{
    public static class ConditionalLogicalOperatorsExamples
    {
        public static void Examples()
        {
            ConditionalAnd();
            ConditionalOr();
        }

        private static void ConditionalAnd()
        {
            // <SnippetAnd>
            bool SecondOperand()
            {
                Console.WriteLine("Second operand is evaluated.");
                return true;
            }

            bool a = false && SecondOperand();
            Console.WriteLine(a);
            // Output:
            // False

            bool b = true && SecondOperand();
            Console.WriteLine(b);
            // Output:
            // Second operand is evaluated.
            // True
            // </SnippetAnd>
        }

        private static void ConditionalOr()
        {
            // <SnippetOr>
            bool SecondOperand()
            {
                Console.WriteLine("Second operand is evaluated.");
                return true;
            }

            bool a = true || SecondOperand();
            Console.WriteLine(a);
            // Output:
            // True

            bool b = false || SecondOperand();
            Console.WriteLine(b);
            // Output:
            // Second operand is evaluated.
            // True
            // </SnippetOr>
        }
    }
}