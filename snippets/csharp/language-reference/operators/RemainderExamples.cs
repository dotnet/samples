using System;

namespace operators
{
    public static class RemainderExamples
    {
        public static void Examples()
        {
            Console.WriteLine("Integer remainder:");
            IntegerRemainder();

            Console.WriteLine("Remainder with binary floating-point types:");
            FloatingPointRemainer();

            Console.WriteLine("Remainder assignment example:");
            RemainderAssignment();
        }

        private static void IntegerRemainder()
        {
            // <Snippet1>
            Console.WriteLine(5 % 4);   // output: 1
            Console.WriteLine(5 % -4);  // output: 1
            Console.WriteLine(-5 % 4);  // output: -1
            Console.WriteLine(-5 % -4); // output: -1
            // </Snippet1>
        }

        private static void FloatingPointRemainer()
        {
            // <Snippet2>
            Console.WriteLine(-5.2f % 2.0f); // output: -1.2
            Console.WriteLine(5.0 % 2.2);    // output: 0.6
            Console.WriteLine(.41 % .2);     // output: 0.00999999999999995
            // </Snippet2>
        }

        private static void RemainderAssignment()
        {
            // <Snippet3>
            int a = 5;
            a %= 3;
            Console.WriteLine(a);  // output: 2
            // </Snippet3>
        }
    }
}