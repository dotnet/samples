using System;

namespace keywords
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================    Stack Alloc Examples ======================");
            StackAllocExamples.Examples();
            Console.WriteLine("=================    Generic Where Constraints Examples ======================");
            GenericWhereConstraints.Examples();
            Console.WriteLine("=================    Fixed Memory Examples ======================");
            FixedKeywordExamples.Examples();
            Console.WriteLine("=================    Iteration Keywords Examples ======================");
            IterationKeywordsExamples.Examples();
            Console.WriteLine("=================    readonly Keyword Examples ======================");
            ReadonlyKeywordExamples.Examples();
            Console.WriteLine("=================    true and false operators examples ==============");
            LaunchStatusTest.Main();
            Console.WriteLine("=================    true and false literals examples ===============");
            TrueFalseLiteralsExamples.Examples();
        }
    }
}
