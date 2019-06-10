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

            Console.WriteLine("============= == and != operators examples =====");
            EqualityOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("======== Logical operators examples ============");
            BooleanLogicalOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("==== Bitwise and shift operators examples ======");
            BitwiseAndShiftOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("====== >, <, >=, and <= operators examples =====");
            ComparisonOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("========= Member access operators examples =====");
            MemberAccessOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("======= Pointer related operators examples =====");
            PointerOperators.Examples();
            Console.WriteLine();

            Console.WriteLine("============== + operator examples =============");
            AdditionOperator.Examples();
            Console.WriteLine();

            Console.WriteLine("============== - operator examples =============");
            SubtractionOperator.Examples();
            Console.WriteLine();

            Console.WriteLine("============== = operator examples =============");
            AssignmentExamples.Examples();
            Console.WriteLine();
            
            Console.WriteLine("============== ?: operator examples ============");
            ConditionalOperator.Examples();
            Console.WriteLine();

            Console.WriteLine("============== ?? operator examples ============");
            NullCoalescingOperator.Examples();
            Console.WriteLine();

            Console.WriteLine("============== () operator examples ============");
            InvocationOperatorExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== => operator examples ============");
            LambdaOperator.Examples();
            Console.WriteLine();

            Console.WriteLine("============ stackalloc operator examples ======");
            StackallocOperator.Examples();
            Console.WriteLine();

            Console.WriteLine("========= true and false operators examples ====");
            LaunchStatusTest.Main();
            Console.WriteLine();
        }
    }
}
