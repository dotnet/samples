using System;

namespace operators
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("============== Remainder operator examples =============");
            RemainderExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== Addition operator examples =============");
            AdditionExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== & operator examples =============");
            AndOperatorExamples.Examples();
            Console.WriteLine();
        }
    }
}
