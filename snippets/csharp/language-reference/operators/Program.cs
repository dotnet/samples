using System;

namespace operators
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======== Arithmetic operators examples =========");
            ArithmeticOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("============== == and != operator examples =====");
            EqualityAndNonEqualityExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("======== Logical operators examples ============");
            LogicalOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("==== Bitwise and shift operators examples ======");
            BitwiseAndShiftOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("============== + operator examples =============");
            AdditionExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== & operator examples =============");
            AndOperatorExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== = operator examples =============");
            AssignmentExamples.Examples();
            Console.WriteLine();
            
            Console.WriteLine("============== ?: operator examples ============");
            ConditionalExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("======= >, <, >=, and <= operator examples =====");
            GreaterAndLessOperatorsExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== [] operator examples ============");
            IndexOperatorExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== () operator examples ============");
            InvocationOperatorExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== => operator examples ============");
            LambdaOperatorExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== . operator examples =============");
            MemberAccessExamples.Examples();
            Console.WriteLine();
        }
    }
}
