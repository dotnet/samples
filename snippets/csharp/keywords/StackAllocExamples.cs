using System;

namespace keywords
{
    public static class StackAllocExamples
    {
        public static void Examples()
        {
            FibonacciGeneration();
            Console.WriteLine("=========================");
            InitializeBitMasks();
        }

        private static unsafe void InitializeBitMasks()
        {
            // <Snippet2>
            int* mask = stackalloc[] {
                0b_0000_0000_0000_0001,
                0b_0000_0000_0000_0010,
                0b_0000_0000_0000_0100,
                0b_0000_0000_0000_1000,
                0b_0000_0000_0001_0000,
                0b_0000_0000_0010_0000,
                0b_0000_0000_0100_0000,
                0b_0000_0000_1000_0000,
                0b_0000_0001_0000_0000,
                0b_0000_0010_0000_0000,
                0b_0000_0100_0000_0000,
                0b_0000_1000_0000_0000,
                0b_0001_0000_0000_0000,
                0b_0010_0000_0000_0000,
                0b_0100_0000_0000_0000,
                0b_1000_0000_0000_0000
            };

            for (int i = 0; i < 16; i++)
                Console.WriteLine(mask[i]);
            /* Output:
               1
               2
               4
               8
               16
               32
               64
               128
               256
               512
               1024
               2048
               4096
               8192
               16384
               32768
             */

            // </Snippet2>
        }

        private static unsafe void FibonacciGeneration()
        {
            // <Snippet1>
            const int arraySize = 20;
            int* fib = stackalloc int[arraySize];
            int* p = fib;
            // The sequence begins with 1, 1.
            *p++ = *p++ = 1;
            for (int i = 2; i < arraySize; ++i, ++p)
            {
                // Sum the previous two numbers.
                *p = p[-1] + p[-2];
            }
            for (int i = 0; i < arraySize; ++i)
            {
                Console.WriteLine(fib[i]);
            }
            /* Output:
               1
               1
               2
               3
               5
               8
               13
               21
               34
               55
               89
               144
               233
               377
               610
               987
               1597
               2584
               4181
               6765
             */
            // </Snippet1>
        }
    }
}
