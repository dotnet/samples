using System;

namespace operators
{
    public static class AndOperatorExamples
    {
        public static void Examples()
        {
            IntegerOperands();
            BooleanOperands();
            AndAssignment();
        }

        private static void IntegerOperands()
        {
            // <SnippetIntegerOperands>
            uint a = 0b_1111_1000;
            uint b = 0b_1001_1111;
            uint c = a & b;
            Console.WriteLine(Convert.ToString(c, toBase: 2));
            // Output:
            // 10011000
            // </SnippetIntegerOperands>
        }

        private static void BooleanOperands()
        {
            // <SnippetBooleanOperands>
            bool SecondOperand() 
            {
                Console.WriteLine("Second operand is evaluated.");
                return true;
            }
            
            bool test = false & SecondOperand();
            Console.WriteLine(test);
            // Output:
            // Second operand is evaluated.
            // False
            // </SnippetBooleanOperands>
        }

        private static void AndAssignment()
        {
            // <SnippetAndAssignmentExample>
            byte a = 0b_1111_1000;
            a &= 0b_1001_1111;
            Console.WriteLine(Convert.ToString(a, toBase: 2));
            // Output:
            // 10011000

            bool b = true;
            b &= false;
            Console.WriteLine(b);
            // Output:
            // False
            // </SnippetAndAssignmentExample>
        }
    }
}