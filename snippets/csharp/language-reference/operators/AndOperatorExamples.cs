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
            byte a = 0b_1111_1000;
            byte b = 0b_1001_1111;
            int c = a & b;
            Console.WriteLine(Convert.ToString(c, toBase: 2));
            // Output:
            // 10011000
            // </SnippetIntegerOperands>
        }

        private static void BooleanOperands()
        {
            // <SnippetBooleanOperands>
            int i = 0;
            bool test = false & (++i == 1);
            Console.WriteLine(test);   // output: False
            Console.WriteLine(i);      // output: 1
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