using System;

namespace operators
{
    public static class BitwiseComplementExamples
    {
        public static void Examples()
        {
            BitwiseComplement();
        }

        private static void BitwiseComplement()
        {
            // <SnippetExample>
            uint a = 0b_0000_1111_0000_1111_0000_1111_0000_0011;
            uint b = ~a;
            Console.WriteLine(Convert.ToString(b, toBase: 2));
            // Output:
            // 11110000111100001111000011111100
            // </SnippetExample>
        }
    }
}