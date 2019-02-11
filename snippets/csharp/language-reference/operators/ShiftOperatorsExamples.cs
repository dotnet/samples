using System;

namespace operators
{
    public static class ShiftOperatorsExamples
    {
        public static void Examples()
        {
            LeftShift();
            LeftShiftByLargeCount();
            LeftShiftAssignment();
            RightShift();
            RightShiftByLargeCount();
            RightShiftAssignment();
        }

        private static void LeftShift()
        {
            // <SnippetLeftShift>
            uint x = 0b_1100_1001_0000_0000_0000_0000_0001_0001;
            uint y = x << 4;
            Console.WriteLine(Convert.ToString(y, toBase: 2));
            // Output:
            // 10010000000000000000000100010000
            // </SnippetLeftShift>
        }

        private static void LeftShiftByLargeCount()
        {
            // <SnippetLeftShiftByLargeCount>
            int a = 0b_0001;
            int count1 = 0b_0000_0001;
            int count2 = 0b_1110_0001;
            Console.WriteLine($"{a} << {count1} is {a << count1}; {a} << {count2} is {a << count2}");
            // Output:
            // 1 << 1 is 2; 1 << 225 is 2
            // </SnippetLeftShiftByLargeCount>
        }

        private static void LeftShiftAssignment()
        {
            // <SnippetLeftShiftAssignment>
            uint x = 0b_1100_1001_0000_0000_0000_0000_0001_0001;
            x <<= 4;
            Console.WriteLine(Convert.ToString(x, toBase: 2));
            // Output:
            // 10010000000000000000000100010000
            // </SnippetLeftShiftAssignment>
        }

        private static void RightShift()
        {
            // <SnippetRightShift>
            uint x = 0b_1001;
            uint y = x >> 2;
            Console.WriteLine(Convert.ToString(y, toBase: 2));
            // Output:
            // 10

            int a = int.MinValue;
            Console.WriteLine($"Before shift: {Convert.ToString(a, toBase: 2)}");
            int count = 3;
            int b = a >> count;
            Console.WriteLine($"Shift right by {count}: {Convert.ToString(b, toBase: 2)}");
            // Output:
            // Before shift: 10000000000000000000000000000000
            // Shift right by 3: 11110000000000000000000000000000
            // </SnippetRightShift>
        }

        private static void RightShiftByLargeCount()
        {
            // <SnippetRightShiftByLargeCount>
            int a = 0b_0100;
            int count1 = 0b_0000_0001;
            int count2 = 0b_1110_0001;
            Console.WriteLine($"{a} >> {count1} is {a >> count1}; {a} >> {count2} is {a >> count2}");
            // Output:
            // 4 >> 1 is 2; 4 >> 225 is 2
            // </SnippetRightShiftByLargeCount>
        }

        private static void RightShiftAssignment()
        {
            // <SnippetRightShiftAssignment>
            uint x = 0b_1001;
            x >>= 2;
            Console.WriteLine(Convert.ToString(x, toBase: 2));
            // Output:
            // 10
            // </SnippetRightShiftAssignment>
        }
    }
}