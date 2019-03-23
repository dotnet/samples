using System;

namespace operators
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("============== % operator examples =============");
            RemainderExamples.Examples();
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

            Console.WriteLine("============== ~ operator examples =============");
            BitwiseComplementExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== && and || operator examples =====");
            ConditionalLogicalOperatorsExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== -- and ++ operator examples =====");
            DecrementAndIncrementExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== / operator examples =============");
            DivisionExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== == and != operator examples =====");
            EqualityAndNonEqualityExamples.Examples();
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

            Console.WriteLine("===== <<, <<=, >>, and >>= operator examples ===");
            ShiftOperatorsExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== ! operator examples =============");
            LogicalNegationExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== . operator examples =============");
            MemberAccessExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== * operator examples =============");
            MultiplicationExamples.Examples();
            Console.WriteLine();
        }
    }
}
